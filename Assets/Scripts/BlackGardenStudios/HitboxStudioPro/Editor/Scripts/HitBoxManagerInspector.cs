﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace BlackGardenStudios.HitboxStudioPro
{
#pragma warning disable 0414
    [CustomEditor(typeof(HitboxManager))]
    public class HitBoxManagerInspector : Editor
    {
        SerializedObject m_Object;
        HitboxManager manager;
        Color oldBGColor;
        Color oldFGColor;
        public bool showColliders;
        public bool showAudio;
        public bool showMove;
        public bool showHit;
        public bool hit_showHitVelocity;
        public bool hit_showBlockVelocity;
        public bool hit_showEnergy;
        public bool hit_otg;
        public bool hit_fall;
        public bool hit_bounce;
        public bool hit_slide;
        public bool hit_wallBounce;
        public bool hit_downedEnemy;
        public bool showAnimation;
        public bool showAttackData;
        public bool showFrameData;
        public bool showLightingData;
        public bool showEvents;

        private SerializedProperty m_SelectedAnimation;
        private SerializedProperty m_SelectedFrame;
        private SerializedProperty m_SelectedCollider;
        private SpriteRenderer m_Renderer;
        List<string> m_AnimationOptionLabels = new List<string>();

        public void OnEnable()
        {
            oldBGColor = GUI.backgroundColor;
            oldFGColor = GUI.contentColor;
            manager = (HitboxManager)target;
            m_Object = new SerializedObject(target);
            m_SelectedAnimation = m_Object.FindProperty("m_CurrentAnimation");
            m_SelectedFrame = m_Object.FindProperty("m_CurrentFrame");
            m_SelectedCollider = m_Object.FindProperty("m_CurrentCollider");
        }

        private void ResetEditorColors()
        {
            GUI.backgroundColor = oldBGColor;
            GUI.contentColor = oldFGColor;
        }

        private void SetEditorColors(Color background, Color content)
        {
            GUI.backgroundColor = background;
            GUI.contentColor = content;
        }

        public void UpdateSerializedObject()
        {
            m_Object.Update();
        }

        public bool ApplySerializedProperties() { return m_Object.ApplyModifiedProperties(); }

        private int SelectedAnimation { get { return m_SelectedAnimation.intValue; } set { m_SelectedAnimation.intValue = value; } }
        private int SelectedFrame { get { return m_SelectedFrame.intValue; } set { m_SelectedFrame.intValue = value; } }
        private int SelectedCollider { get { return manager.m_CurrentCollider; } set { manager.m_CurrentCollider = value; } }
        private HitboxManager.HitboxAnimation[] Animation { get { return manager.m_Animations; } }

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                GUILayout.Label("Cannot edit animations while the game is running.", EditorStyles.miniTextField);
                return;
            }

            UpdateSerializedObject();
            
            HitboxManager m_Target = (HitboxManager)target;
            SerializedProperty Animations = m_Object.FindProperty("m_Animations");

            m_AnimationOptionLabels.Clear();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Separator();
            if (m_Target.m_Animations != null && m_Target.m_Animations.Length > 0)
            {
                for (int i = 0; i < m_Target.m_Animations.Length; i++)
                {
                    if (m_Target.m_Animations[i].clip == null)
                    {
                        m_AnimationOptionLabels.Add("undefined_" + i);
                        continue;
                    }
                    m_AnimationOptionLabels.Add(m_Target.m_Animations[i].clip.name);
                }
            }
            else
                m_AnimationOptionLabels.Add("none");
            #region BUTTONS
            if (GUILayout.Button("Add New Animation"))
            {
                string path = EditorUtility.OpenFilePanel("Select Animation Clip", "", "anim");

                if(!string.IsNullOrEmpty(path))
                {
                    int indexOf = path.IndexOf("Assets/");
                    path = path.Substring(indexOf >= 0 ? indexOf : 0);
                    var loadedClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);

                    if(loadedClip == null)
                    {
                        Debug.LogError("HITBOXMANAGER ERROR: Unable to load animation clip at path \"" + path + "\"\n\r Is this this animation located inside your project's assets folder?");
                    }
                    else
                    {
                        Animations.InsertArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));

                        m_Object.ApplyModifiedProperties();

                        SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(Animations.arraySize - 1);
                        var clip = HitboxData.FindPropertyRelative("clip");
                        var framedata = HitboxData.FindPropertyRelative("framedata");

                        clip.objectReferenceValue = loadedClip;
                        for (int i = 0, j = m_Target.GetNumFrames(Animations.arraySize - 1); i < j; i++)
                        {
                            framedata.InsertArrayElementAtIndex(0);
                            framedata.GetArrayElementAtIndex(0).FindPropertyRelative("collider").ClearArray();
                        }

                        SelectedAnimation = Animations.arraySize - 1;
                        SelectedFrame = 0;
                        m_Object.ApplyModifiedProperties();
                    }
                }
            }
            SetEditorColors(Color.yellow, Color.white);
            if (GUILayout.Button("Add All Animations From Attached Animator"))
            {
                var animator = m_Target.GetComponent<Animator>();
                var allclips = animator.runtimeAnimatorController.animationClips;
                var allclipslist = new List<AnimationClip>(allclips);

                for (int i = 0; i < Animations.arraySize; i++)
                {
                    var currentclip = (AnimationClip)Animations.GetArrayElementAtIndex(i).FindPropertyRelative("clip").objectReferenceValue;

                    allclipslist.Remove(currentclip);
                }

                for (int i = 0; i < allclipslist.Count; i++)
                {
                    Animations.InsertArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));

                    m_Object.ApplyModifiedProperties();

                    SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(Animations.arraySize - 1);
                    var clip = HitboxData.FindPropertyRelative("clip");
                    var framedata = HitboxData.FindPropertyRelative("framedata");

                    clip.objectReferenceValue = allclipslist[i];
                    for (int k = 0, j = m_Target.GetNumFrames(Animations.arraySize - 1); k < j; k++)
                    {
                        framedata.InsertArrayElementAtIndex(0);
                        framedata.GetArrayElementAtIndex(0).FindPropertyRelative("collider").ClearArray();
                    }
                    m_Object.ApplyModifiedProperties();
                }
            }
            SetEditorColors(Color.red, Color.white);
            if (GUILayout.Button("Remove Current Animation") &&
                EditorUtility.DisplayDialog("Remove " + m_AnimationOptionLabels[SelectedAnimation] + " from " + m_Target.transform.root.name + "?",
                    "Are you sure you want to remove this animation from " + m_Target.transform.root.name + "? All collider data, movement data, and other custom data for this animation on this character will be deleted.", "Yes", "No"))
            {
                Animations.MoveArrayElement(SelectedAnimation, Mathf.Max(0, Animations.arraySize - 1));
                Animations.DeleteArrayElementAtIndex(Mathf.Max(0, Animations.arraySize - 1));
                SelectedAnimation = Mathf.Max(0, SelectedAnimation - 1);
            }
            ResetEditorColors();
            #endregion

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            var oldSelectedAnim = SelectedAnimation;
            SelectedAnimation = EditorGUILayout.Popup("Select animation to edit:",
                SelectedAnimation, m_AnimationOptionLabels.ToArray(), EditorStyles.popup);

            if (oldSelectedAnim != SelectedAnimation)
            {
                SelectedFrame = 0;

                //We've changed animations so lets clear any AnimationEvent from the selection.
                if (Selection.activeObject is AnimationEventObject)
                    Selection.SetActiveObjectWithContext(null, null);
            }

            EditorGUILayout.Separator();

            if (SelectedAnimation < Animations.arraySize)
            {
                SerializedProperty HitboxData = Animations.GetArrayElementAtIndex(SelectedAnimation);

                if (HitboxData != null)
                {
                    var clip = HitboxData.FindPropertyRelative("clip");
                    var framedata = HitboxData.FindPropertyRelative("framedata");
                    var oldclip = clip.objectReferenceValue;

                    /*showAnimation = EditorGUILayout.Foldout(showAnimation, "Animation Settings", true);
                    if (showAnimation)
                    {
                        var movementstate = HitboxData.FindPropertyRelative("movementstate");
                        var movementspeed = HitboxData.FindPropertyRelative("movementspeed");
                        var poise = HitboxData.FindPropertyRelative("poise");

                        EditorGUI.indentLevel++;

                        EditorGUILayout.PropertyField(clip);
                        EditorGUILayout.PropertyField(movementstate, new GUIContent("Input State", "Controls whether or not this animations enables or disables player inputs"));
                        EditorGUILayout.PropertyField(movementspeed, new GUIContent("Speed Multiplier", "Sets the move speed during this animation relative to their base speed (0.0 - 1.0)"));
                        EditorGUILayout.PropertyField(poise, new GUIContent("Additional Poise", "The more poise a character has, the less likely they will be stagerred or knocked back."));

                        EditorGUI.indentLevel--;
                    }*/

                    showAttackData = EditorGUILayout.Foldout(showAttackData, "Attack Data", true);
                    if (showAttackData)
                    {
                        EditorGUI.indentLevel++;
                        
                        var numhits = HitboxData.FindPropertyRelative("numhits");

                        //EditorGUILayout.PropertyField(strength, new GUIContent("Poise Damage", "The amount of poise that will be taken from the defender when hit by this attack."));
                        //EditorGUILayout.PropertyField(numtargets, new GUIContent("Cleave Strength", "Number of enemies this attack can hit."));
                        //EditorGUILayout.PropertyField(force, new GUIContent("Force Power", "The minimum (X) and maximum (Y) amount of force to apply to the target hit."), true);
                        //EditorGUILayout.LabelField(new GUIContent("Force Direction", "Direction vector of force to apply, random between Min and Max."));
                        //EditorGUILayout.BeginHorizontal();
                        //EditorGUI.indentLevel++;
                        //EditorGUILayout.PropertyField(mindirection, new GUIContent("Min", "Minimum direction vector to apply force."),
                        //    true, GUILayout.MaxWidth(130f));
                        //EditorGUILayout.PropertyField(maxdirection, new GUIContent("Max", "Maximum direction vector to apply force."),
                        //    true, GUILayout.MaxWidth(130f));
                        //EditorGUI.indentLevel--;
                        //EditorGUILayout.EndHorizontal();

                        EditorGUILayout.PropertyField(numhits, new GUIContent("Number of Hits", "The maximum number of hits this attack can make. Set to -1 for unlimited hits."));

                        EditorGUI.indentLevel--;
                    }

                    //New animations need their frame data populated
                    if (oldclip != clip.objectReferenceValue && clip.objectReferenceValue != null || framedata.arraySize == 0)
                    {
                        framedata.ClearArray();
                        framedata.arraySize = 0;
                        ApplySerializedProperties();
                        for (int i = 0, j = m_Target.GetNumFrames(SelectedAnimation); i < j; i++)
                        {
                            framedata.InsertArrayElementAtIndex(0);
                        }
                        ApplySerializedProperties();
                    }

                    showFrameData = EditorGUILayout.Foldout(showFrameData, "Frame Data", true);
                    if (showFrameData)
                    {
                        GUILayout.Label("Frames in clip: " + m_Target.GetNumFrames(SelectedAnimation));
                        SelectedFrame = EditorGUILayout.IntSlider(SelectedFrame, 0, Mathf.Max(0, m_Target.GetNumFrames(SelectedAnimation) - 1));

                        EditorGUI.indentLevel++;

                        if (SelectedFrame < framedata.arraySize)
                        {
                            var collider = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("collider");
                            if (collider != null)
                            {
                                //TODO: movement data isnt showing up
                                showMove = EditorGUILayout.Foldout(showMove, "Movement Data", true);
                                if (showMove)
                                {
                                    EditorGUI.indentLevel++;

                                    var capsule = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("capsuleOffset");
                                    var smoothing = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("smoothedOffset");

                                    if (capsule != null)
                                    {
                                        EditorGUILayout.PropertyField(capsule, new GUIContent("Move By", "Move this character in the world by this many pixels"), true);
                                        if (smoothing != null)
                                        {
                                            EditorGUILayout.PropertyField(smoothing, new GUIContent("Smoothed Move", "A smoothed movement will smoothly interpolate this movement. ie sliding instead of instantly moving."));
                                        }
                                    }

                                    EditorGUI.indentLevel--;
                                }
                            }

                            /**New Fields**/
                            showHit = EditorGUILayout.Foldout(showHit, "Hit Data", true);
                            if (showHit)
                            {
                                EditorGUI.indentLevel++;

                                var damage  = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("damage");
                                var minDamage  = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("minDamage");
                                var chipDamage =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("chipDamage");
                                var guardType =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("guardType");
                                var attackPriority =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("attackPriority");
                                var hitpause  = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitpause");
                                var blockpause  = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockpause");
                                var hitstun =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitstun");
                                var blockstun =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockstun");

                                var hitGroundVelocity =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitGroundVelocity");
                                var hitGroundVelocityTime =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitGroundVelocityTime");
                                var hitAirVelocity =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitAirVelocity");
                                var hitAirVelocityTime =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitAirVelocityTime");

                                var blockGroundVelocity =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockGroundVelocity");
                                var blockGroundVelocityTime =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockGroundVelocityTime");
                                var blockAirVelocity =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockAirVelocity");
                                var blockAirVelocityTime =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("blockAirVelocityTime");
                                
                                var giveSelfPower = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("giveSelfPower");
                                var giveEnemyPower = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("giveEnemyPower");
                                
                                var downedHit = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downedHit");
                                var downedDamage = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downedDamage");
                                var downedHitstun = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downedHitstun");
                                var downedVelocity = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downedVelocity");
                                
                                var fallingGravity = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallingGravity");
                                var fallAir = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallAir");
                                var fallGround = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallGround");
                                var fallRecover = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallRecover");
                                
                                var bounce = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("bounce");
                                var bounceGravity = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("bounceGravity");
                                var bounceRecover = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("bounceRecover");
                                
                                var slide = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("slide");
                                var slideTime = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("slideTime");
                                
                                var wallBounce = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("wallBounce");
                                var wallBounceGravity = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("wallBounceGravity");
                                var wallBounceSlide = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("wallBounceSlide");
                                var wallBounceTime = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("wallBounceTime");
                                
                                var downTime = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downTime");
                                var downRecover = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("downRecover");
                                
                                var hitShakeTime = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitShakeTime");
                                var hitShakeX = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitShakeX");
                                var hitShakeY = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitShakeY");
                                var fallShakeTime = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallShakeTime");
                                var fallShakeX = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallShakeX");
                                var fallShakeY = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("fallShakeY");
                                
                                var forceStand = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("forceStand");
                                var flipEnemy = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("flipEnemy");
                                
                                EditorGUILayout.PropertyField(damage, new GUIContent("Attack Damage", "The amount of damage this hit will deal when it hits."));
                                EditorGUILayout.PropertyField(minDamage, new GUIContent("Min. Attack Damage", "The minimum amount of damage this hit will deal after damage scaling."));
                                EditorGUILayout.PropertyField(chipDamage, new GUIContent("Chip Damage", "The amount of damage this hit will deal when it is guarded."));
                                EditorGUILayout.PropertyField(guardType, new GUIContent("Guard Type",
                                    "The type of block the enemy must make to block the hit.\n"+
                                    "MID: The hit can be blocked crouching or not crouching\n"+
                                    "LOW: Must be blocked while crouching\n"+
                                    "HIGH: Must be blocked while not crouching\n"+
                                    "UNBLOCKABLE: The hit cannot be blocked"
                                ));
                                EditorGUILayout.PropertyField(attackPriority, new GUIContent("Attack Priority",
                                    "The priority/strength of the hit compared to other attacks.\n"+
                                    "Priorities are listed in ascending order of strength."
                                ));
                                EditorGUILayout.PropertyField(hitpause, new GUIContent("Hit Pause", "If hit, the game pauses for this many ticks."));
                                EditorGUILayout.PropertyField(blockpause, new GUIContent("Block Pause", "If blocked, the game pauses for this many ticks."));
                                EditorGUILayout.PropertyField(hitstun, new GUIContent("Hit Stun", "If hit, the enemy will remain in the hit state for this amount of ticks."));
                                EditorGUILayout.PropertyField(blockstun, new GUIContent("Block Stun", "If blocked, the enemy will remain in the blocking state for this amount of ticks."));
                                
                                hit_showHitVelocity = EditorGUILayout.Foldout(hit_showHitVelocity, "Velocity on Hit", true);
                                if (hit_showHitVelocity) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(hitGroundVelocity, new GUIContent("Ground Velocity", "If the enemy is on the ground, the enemy is set to this velocity."));
                                    EditorGUILayout.PropertyField(hitGroundVelocityTime, new GUIContent("G.Velocity Time", "The enemy remains with the above velocity for this amount of ticks."));
                                    EditorGUILayout.PropertyField(hitAirVelocity, new GUIContent("Air Velocity", "If the enemy is on the ground, the enemy is set to this velocity."));
                                    EditorGUILayout.PropertyField(hitAirVelocityTime, new GUIContent("A.Velocity Time", "The enemy remains with the above velocity for this amount of ticks."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_showBlockVelocity = EditorGUILayout.Foldout(hit_showBlockVelocity, "Velocity on Block", true);
                                if (hit_showBlockVelocity) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(blockGroundVelocity, new GUIContent("Ground Velocity", "If the enemy is on the ground and blocks the hit, the enemy is set to this velocity."));
                                    EditorGUILayout.PropertyField(blockGroundVelocityTime, new GUIContent("G.Velocity Time", "The enemy remains with the above velocity for this amount of ticks."));
                                    EditorGUILayout.PropertyField(blockAirVelocity, new GUIContent("Air Velocity", "If the enemy is on the ground and blocks the hit, the enemy is set to this velocity."));
                                    EditorGUILayout.PropertyField(blockAirVelocityTime, new GUIContent("A.Velocity Time", "The enemy remains with the above velocity for this amount of ticks."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_showEnergy = EditorGUILayout.Foldout(hit_showEnergy, "Get/Give Energy", true);
                                if (hit_showEnergy) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(giveSelfPower, new GUIContent("Get Energy", "On hit, the character hitting gets this much power."));
                                    EditorGUILayout.PropertyField(giveEnemyPower, new GUIContent("Give Energy", "On hit, the character getting hit gets this much power."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_downedEnemy = EditorGUILayout.Foldout(hit_downedEnemy, "Downed options", true);
                                if (hit_downedEnemy) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(downTime, new GUIContent("Down time", "The amount of time the player is left in the lying down state."));
                                    EditorGUILayout.PropertyField(downRecover, new GUIContent("Recover", "Whether the player can recover from the lying down state differently."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_otg = EditorGUILayout.Foldout(hit_otg, "OTG (Off-the-Ground)", true);
                                if (hit_otg) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(downedHit, new GUIContent("OTG", "Whether this hit should hit enemies that are in downed states."));
                                    EditorGUILayout.PropertyField(downedDamage, new GUIContent("OTG Damage", "Damage dealt when the enemy is hit while downed."));
                                    EditorGUILayout.PropertyField(downedHitstun, new GUIContent("OTG Hitstun", "Hitstun given when the enemy is hit while downed."));
                                    EditorGUILayout.PropertyField(downedVelocity, new GUIContent("OTG Velocity", "The velocity of the enemy when hit while downed."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_fall = EditorGUILayout.Foldout(hit_fall, "Falling", true);
                                if (hit_fall) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(fallGround, new GUIContent("Fall off of Ground",
                                        "If true, and the enemy is hit into the air, the enemy will continue to fall after hitstun.\n" +
                                        "The hit enemy's gravity is overrided by the fall gravity of this hit."
                                    ));
                                    EditorGUILayout.PropertyField(fallAir, new GUIContent("Fall in Air",
                                        "If true, and the enemy was hit while in air, the enemy will continue to fall after hitstun.\n" +
                                        "The hit enemy's gravity is overrided by the fall gravity of this hit."
                                    ));
                                    EditorGUILayout.PropertyField(fallingGravity, new GUIContent("Fall Gravity", "If not zero, the new fall gravity of the hit enemy."));
                                    EditorGUILayout.PropertyField(fallRecover, new GUIContent("Recover", "Whether the enemy can recover out of the fall state or not."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_bounce = EditorGUILayout.Foldout(hit_bounce, "Ground Bounce", true);
                                if (hit_bounce) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(bounce, new GUIContent("Velocity", "When the enemy first lands on the ground after falling, this velocity is applied."));
                                    EditorGUILayout.PropertyField(bounceGravity, new GUIContent("Gravity", "If not zero, this gravity is applied to the enemy when in the bounce state."));
                                    EditorGUILayout.PropertyField(bounceRecover, new GUIContent("Recover", "Whether the enemy can recover out of the bounce state or not."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_slide = EditorGUILayout.Foldout(hit_slide, "Ground Slide", true);
                                if (hit_slide) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(slide, new GUIContent("Velocity", "When the enemy comes down from a ground bounce, this velocity is applied."));
                                    EditorGUILayout.PropertyField(slideTime, new GUIContent("Time", "The amount of time the above velocity is applied. After this time exprires, the enemy enters the lying down state."));
                                    EditorGUI.indentLevel--;
                                }

                                hit_wallBounce = EditorGUILayout.Foldout(hit_wallBounce, "Wall Bounce", true);
                                if (hit_wallBounce) {
                                    EditorGUI.indentLevel++;
                                    EditorGUILayout.PropertyField(wallBounceTime, new GUIContent("Slide Time", "If not zero, the amount of ticks the enemy is stuck to the wall for if hit towards it."));
                                    EditorGUILayout.PropertyField(wallBounceSlide, new GUIContent("Slide", "The speed at which the enemy slides down the wall while on it"));
                                    EditorGUILayout.PropertyField(wallBounce, new GUIContent("Velocity", "The velocity the enemy comes off of the wall at."));
                                    EditorGUILayout.PropertyField(wallBounceGravity, new GUIContent("Gravity", "If not zero, this gravity is applied to the enemy after coming off the wall."));
                                    EditorGUI.indentLevel--;
                                }

                                EditorGUILayout.PropertyField(forceStand, new GUIContent("Force Stand", "When this attack lands and the enemy is not in the air, the enemy is forced into the standing hurt."));
 
                                var hitfxlabel = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitfxlabel");
                                var hitfxuid = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("hitfxuid");
                                var effects = EffectSpawner.GetPools();
                                var effectLabels = new List<string>();

                                effectLabels.Add("None");

                                for (int i = 0; i < effects.Length; i++) {
                                    if (effects[i].uniqueID == hitfxuid.intValue) {
                                        hitfxlabel.intValue = i + 1;
                                    }
                                    effectLabels.Add(effects[i].name);
                                }

                                hitfxlabel.intValue = EditorGUILayout.Popup("Hit Effect Pool",
                                    hitfxlabel.intValue, effectLabels.ToArray(), EditorStyles.popup);

                                if (hitfxlabel.intValue > 0)
                                    hitfxuid.intValue = effects[hitfxlabel.intValue - 1].uniqueID;
                                else
                                    hitfxuid.intValue = 0;


                                var soundfxlabel = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("soundfxlabel");
                                var soundfxuid = framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("soundid");
                                var soundEffects = EffectSpawner.GetSoundPools();
                                var soundLabels = new List<string>();

                                soundLabels.Add("None");

                                for (int i = 0; i < soundEffects.Length; i++) {
                                    if (soundEffects[i].uniqueID == soundfxlabel.intValue) {
                                        soundfxlabel.intValue = i + 1;
                                    }
                                    soundLabels.Add(soundEffects[i].name);
                                }

                                soundfxlabel.intValue = EditorGUILayout.Popup("Sound Effect Pool",
                                    soundfxlabel.intValue, soundLabels.ToArray(), EditorStyles.popup);

                                if (soundfxlabel.intValue > 0)
                                    soundfxuid.intValue = soundEffects[soundfxlabel.intValue - 1].uniqueID;
                                else
                                    soundfxuid.intValue = 0;

                                var stopSound =  framedata.GetArrayElementAtIndex(SelectedFrame).FindPropertyRelative("stopSound");
                                EditorGUILayout.PropertyField(stopSound, new GUIContent("Stop Sounds", "If hit and a sound plays, all other hit sounds are stopped for the sound to play if this is true."));

                                
                                EditorGUI.indentLevel--;
                            }
                            /**************/

                        }
                        else
                        {
                            for (int i = framedata.arraySize; i <= SelectedFrame; i++)
                            {
                                framedata.InsertArrayElementAtIndex(i);
                                ApplySerializedProperties();
                            }
                        }

                        SetEditorColors(Color.yellow, Color.black);
                        if (GUILayout.Button(new GUIContent("Repair Unordered Frames", "Sometimes Unity does not properly sort keyframes by time. This will present some nasty side effects when that animation is used in Hitbox Studio Pro. This will sort those keyframes on the sprite curve properly.")))
                        {
                            var curves = AnimationUtility.GetObjectReferenceCurveBindings(m_Target.m_Animations[SelectedAnimation].clip);

                            for (int i = 0; i < curves.Length; i++)
                                if (curves[i].propertyName.Equals("m_Sprite"))
                                {
                                    var keyframes = AnimationUtility.GetObjectReferenceCurve(m_Target.m_Animations[SelectedAnimation].clip, curves[i])
                                                    .OrderBy((x) => x.time).ToArray();

                                    AnimationUtility.SetObjectReferenceCurve(m_Target.m_Animations[SelectedAnimation].clip, curves[i], keyframes);
                                }
                        }
                        ResetEditorColors();

                        EditorGUI.indentLevel--;
                    }
                }
            }

            EditorGUILayout.EndVertical();

            var maxHitboxes = 1;
            if (Animation != null)
                for (int i = 0; i < Animation.Length; i++)
                    if (Animation[i].framedata != null)
                        for (int j = 0; j < Animation[i].framedata.Length; j++)
                            if(Animation[i].framedata[j].collider != null)
                                maxHitboxes = Mathf.Max(maxHitboxes, Animation[i].framedata[j].collider.Length);

            m_Object.FindProperty("m_MaxHitboxes").intValue = maxHitboxes;
            ApplySerializedProperties();

            if (Event.current.type != EventType.Repaint && Event.current.type != EventType.MouseMove && Event.current.type != EventType.Layout)
            {
                Profiler.BeginSample("Hitbox Inspector: Bake Animation");
                if (SelectedAnimation < Animations.arraySize)
                    m_Target.BakeAnimation(SelectedAnimation);
                //m_Target.BakeAnimations();
                Profiler.EndSample();
            }

            UpdatePreview();

            EditorGUI.indentLevel = 0;
        }

        private int m_LastAnimationID = int.MaxValue, m_LastFrameID = int.MaxValue;

        private void UpdatePreview()
        {
            if(m_LastAnimationID != SelectedAnimation || m_LastFrameID != SelectedFrame)
                manager.UpdatePreview();

            m_LastAnimationID = SelectedAnimation;
            m_LastFrameID = SelectedFrame;


        }

        #region Virtual Canvas Editor

        #region Properties
        /// <summary>
        /// Setup a matrix of values to multiply by; so dragging moves the rect properly
        /// Starts at top left, then travels clockwise, and center is last
        /// </summary>
        private EditorHandle[] m_ColliderDragHandle = new EditorHandle[9]
            {
            new EditorHandle(new Vector4(1f, 0f, -1f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 0f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 1f, -1f)),
            new EditorHandle(new Vector4(0f, 0f, 1f, 0f)),
            new EditorHandle(new Vector4(0f, -1f, 1f, 1f)),
            new EditorHandle(new Vector4(0f, -1f, 0f, 1f)),
            new EditorHandle(new Vector4(1f, -1f, -1f, 1f)),
            new EditorHandle(new Vector4(1f, 0f, -1f, 0f)),
            new EditorHandle(new Vector4(1f, -1f, 0f, 0f))
            };

        /// <summary>
        /// Just assuming the user will never have more than 12 gizmos in a single frame here. If you're an absolute madman you can increase this.
        /// </summary>
        private EditorHandle[] m_GizmoDragHandle = new EditorHandle[24]
        {
        new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),
        new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),
        new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),
        new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle(),new EditorHandle()
        };

        private EditorHandle m_ActiveHandle = null;
        private EditorHandle m_MouseOverHandle = null;
        private EditorHandle m_PivotHandle = new EditorHandle();

        private string[] m_TypeLabels = Enum.GetNames(typeof(HitboxType));
        private string[] m_EventLabels = Enum.GetNames(typeof(FrameEvent));
        private bool m_EditorMenuOpen = false;
        private Rect m_EditorMenuRect = Rect.zero;
        private List<EditorMenuItem> m_EditorMenuItem = new List<EditorMenuItem>(18);
        private float m_EditorScale = 1f;
        private Vector2 m_PreviewOrigin = Vector2.zero;
        private bool m_DragTimeline = false;
        #endregion

        private void RepairFrameData()
        {
            var length = manager.GetNumFrames(SelectedAnimation);
            var diff = length - manager.m_Animations[SelectedAnimation].framedata.Length;

            if (diff != 0)
            {
                var list = new List<HitboxManager.HitboxAnimationFrame>(manager.m_Animations[SelectedAnimation].framedata);

                while (diff < 0)
                {
                    list.RemoveAt(list.Count - 1);
                    diff++;
                }

                while (diff > 0)
                {
                    list.Add(new HitboxManager.HitboxAnimationFrame { collider = new HitboxManager.HitboxColliderData[0] });
                    diff--;
                }

                manager.m_Animations[SelectedAnimation].framedata = list.ToArray();
            }
            else
            {
                var curves = AnimationUtility.GetObjectReferenceCurveBindings(manager.m_Animations[SelectedAnimation].clip);

                for (int i = 0; i < curves.Length; i++)
                    if (curves[i].propertyName.Equals("m_Sprite"))
                    {
                        var keyframes = AnimationUtility.GetObjectReferenceCurve(manager.m_Animations[SelectedAnimation].clip, curves[i]);

                        for (int j = 0; j < manager.m_Animations[SelectedAnimation].framedata.Length && j < keyframes.Length; j++)
                            manager.m_Animations[SelectedAnimation].framedata[j].time = keyframes[j].time;
                    }
            }
        }

        public void DrawEditorTimeline(Vector2 position, float width, Vector2 mouse)
        {
            UpdateSerializedObject();

            if (Animation == null || SelectedAnimation >= Animation.Length ||
            Animation[SelectedAnimation].framedata == null || SelectedFrame >= Animation[SelectedAnimation].framedata.Length) return;
            RepairFrameData();
            var animation = Animation[SelectedAnimation];
            var frames = animation.framedata;
            var length = frames.Length;
            var eventType = Event.current.type;
            var eventButton = Event.current.button;

            if (m_EditorMenuOpen == true && eventType == EventType.MouseDown && eventButton == 0)
            {
                if (m_EditorMenuRect.Contains(mouse))
                {
                    for (int i = 0; i < m_EditorMenuItem.Count; i++)
                        m_EditorMenuItem[i].ProcessEvent(mouse, eventType, eventButton);
                    //Since they clicked inside the menu we're going to assume they chose an option
                    //and return so we don't accidentally click something beneath the menu.
                    Event.current.button = 3;
                    return;
                }
                else
                {
                    m_EditorMenuOpen = false;
                    return;
                }
            }

            if (eventType == EventType.Repaint)
            {
                DrawLine(position, position + new Vector2(width, 0f), 6, Color.black);
                DrawLine(position, position + new Vector2(width, 0f), 4, Color.gray);
            }

            for (int i = 0; i < length; i++)
            {
                var normalizedTime = frames[i].time / animation.clip.length;
                var frameRect = new Rect(position.x + normalizedTime * width, position.y - 5, 10, 10);
                bool frameContains = frameRect.Contains(mouse);

                if (eventType == EventType.Repaint)
                {
                    EditorGUI.DrawRect(frameRect, frameContains ? new Color(0.2f, 0.05f, 0.2f, 1f) : Color.black);
                    EditorGUI.DrawRect(new Rect(position.x + normalizedTime * width + 1, position.y - 4, 8, 8),
                        frameContains ? Color.cyan : (SelectedFrame == i ? Color.yellow : Color.white));
                    EditorGUIUtility.AddCursorRect(frameRect, MouseCursor.Link);
                }
                else if (frameContains == true && eventType == EventType.MouseDown)
                {
                    if (eventButton == 0)
                    {
                        SelectedFrame = i;
                        m_DragTimeline = true;
                    }
                    else if (eventButton == 1)
                        CreateEditFrameMenu(frames[i].time, mouse);
                    else if(eventButton == 2)
                        Animation[SelectedAnimation].framedata[i].skipColliderUpdate = !Animation[SelectedAnimation].framedata[i].skipColliderUpdate;
                }
                else if (eventType == EventType.MouseUp && eventButton == 0)
                    m_DragTimeline = false;
                else if (m_DragTimeline == true && frameContains == true && eventType == EventType.MouseDrag && eventButton == 0)
                    SelectedFrame = i;

                if (eventType == EventType.Repaint && frames[i].collider != null)
                {
                    var count = frames[i].collider.Length;

                    DrawLabel(frames[i].skipColliderUpdate ? ">" : count.ToString(), new Vector2(position.x + 2f + normalizedTime * width, position.y - 15), 8);
                }
            }

            //Draw END frame
            {
                var frameRect = new Rect(position.x + width - 5, position.y - 5, 25, 10);
                bool frameContains = frameRect.Contains(mouse);

                if (eventType == EventType.Repaint)
                {
                    DrawEdgeLabel("END", frameRect.position - new Vector2(0, 2f), frameContains ? Color.cyan : (SelectedFrame == length - 1 ? Color.yellow : Color.white), Color.black, 10, FontStyle.Bold);
                    EditorGUIUtility.AddCursorRect(frameRect, MouseCursor.Link);
                }                    
                else if (frameContains == true && eventType == EventType.MouseDown)
                {
                    if (eventButton == 0)
                    {
                        SelectedFrame = length - 1;
                        m_DragTimeline = true;
                    }
                    else if(eventButton == 1)
                        CreateEditFrameMenu(animation.clip.length, mouse);
                }
            }

            //Handle animation events
            var events = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(animation.clip));
            var selectedEvent = Selection.activeObject as AnimationEventObject;
            var timeDict = new Dictionary<float, int>(events.Count);
            for (int i = 0; i < events.Count; i++)
            {
                //Hide events that are automatically managed by Hitbox Studio Pro
                if (events[i].functionName.StartsWith("_EVENT_") || events[i].functionName.Equals("UpdateHitbox")) continue; 

                if (timeDict.ContainsKey(events[i].time) == false)
                    timeDict.Add(events[i].time, 0);

                var y = ++timeDict[events[i].time];
                var normalizedTime = events[i].time / animation.clip.length;
                var eventRect = new Rect(position.x + normalizedTime * width, position.y - 1 + 10 * y, 5, 5);
                bool eventContains = eventRect.Contains(mouse);

                if (eventType == EventType.Repaint)
                {
                    EditorGUI.DrawRect(eventRect, eventContains ? new Color(0.2f, 0.05f, 0.2f, 1f) : Color.black);
                    EditorGUI.DrawRect(new Rect(position.x + normalizedTime * width + 1, position.y + 10 * y, 4, 4),
                        eventContains ? Color.cyan : (selectedEvent != null && selectedEvent.eventIndex == i ? Color.yellow : Color.white));

                    EditorGUIUtility.AddCursorRect(eventRect, MouseCursor.ArrowMinus);

                    if (eventContains)
                    {
                        var label = events[i].functionName.IndexOf("EVENT_") == 0 ? events[i].functionName.Substring(6) : events[i].functionName;
                        var labelLength = label.Length * 5f;
                        var viewWidth = EditorGUIUtility.currentViewWidth;

                        if (eventRect.position.x + 10 + labelLength > viewWidth)
                            eventRect.position -= new Vector2(labelLength + 15, 0);
                        DrawLabel(label, new Vector2(eventRect.position.x + 10f, eventRect.position.y), 8);
                    }
                }
                else if (eventContains == true && eventType == EventType.MouseDown)
                {
                    if(eventButton == 0)
                    {
                        Selection.SetActiveObjectWithContext(AnimationEventObject.Edit(animation.clip, i), null);
                    }
                    else if(eventButton == 1)
                    {
                        //Make sure we clear the selection if it's an AnimationEventObject
                        if (selectedEvent != null)
                            Selection.SetActiveObjectWithContext(null, null);
                        //Remove event at current index.
                        events.RemoveAt(i--);
                        AnimationUtility.SetAnimationEvents(animation.clip, events.ToArray());
                        //Consume the button event
                        eventButton = -1;
                        //If we accidentally opened a menu while deleting this event lets close it.
                        if (m_EditorMenuOpen)
                            CloseMenu();
                    }
                }
            }
            
            if (eventType == EventType.Repaint)
            {
                DrawLabel("Colliders", new Vector2(position.x - 40, position.y - 15), 8);
                DrawLabel("Frame", new Vector2(position.x - 40, position.y - 5), 8);
                DrawLabel("Events", new Vector2(position.x - 40, position.y + 5), 8);

                //Draw right click menu
                if (m_EditorMenuOpen)
                {
                    EditorGUI.DrawRect(new Rect(m_EditorMenuRect.x - 1,
                                                m_EditorMenuRect.y - 1,
                                                m_EditorMenuRect.width + 2,
                                                m_EditorMenuRect.height + 2), Color.black);

                    for (int i = 0; i < m_EditorMenuItem.Count; i++)
                        m_EditorMenuItem[i].Draw(mouse);
                }
            }

            ApplySerializedProperties();
        }

        public void DrawEditorGizmos(Vector2 position, Vector2 pivot, Vector2 mouse, float scale)
        {
            UpdateSerializedObject();

            if (Animation == null || SelectedAnimation >= Animation.Length ||
            Animation[SelectedAnimation].framedata == null || SelectedFrame >= Animation[SelectedAnimation].framedata.Length) return;
            var animation = Animation[SelectedAnimation];
            var framedata = animation.framedata[SelectedFrame];
            var eventType = Event.current.type;
            var eventButton = Event.current.button;
            var events = AnimationUtility.GetAnimationEvents(Animation[SelectedAnimation].clip);
            var isLastFrame = animation.framedata.Length - 1 == SelectedFrame;

            m_EditorScale = scale;
            m_PreviewOrigin = position;

            if (events != null)
            {
                int currGizmo = 0;

                for (int i = 0; i < events.Length; i++)
                {
                    if ((events[i].time == framedata.time || isLastFrame && events[i].time > framedata.time) 
                        && events[i].functionName.Contains("PROJECTILE"))
                    {
                        var origin = HitboxManager.DecodeIntToVector2(events[i].intParameter) * scale;
                        var dest = HitboxManager.DecodeIntToVector2((int)events[i].floatParameter) * scale;

                        m_MouseOverHandle = null;
                        //If the dots are overlapping we're going to force them apart
                        if (dest == origin)
                            dest.x += 20 * scale;

                        if (eventType == EventType.Repaint)
                        {
                            var a = new Vector2(-1f, 1f);
                            var b = new Vector2(-1f, -1f);
                            var direction = Vector3.Normalize(dest - origin);
#if UNITY_2017_1_OR_NEWER
                            var rotation = Quaternion.AngleAxis(Vector2.SignedAngle(direction, Vector2.right), -Vector3.forward);
#else
                            var angle = Vector2.Angle(direction, Vector2.right);
                            var cross = Vector3.Cross(direction, Vector3.right);
                            var rotation = Quaternion.AngleAxis(cross.z < 0 ? -angle : angle, -Vector3.forward);
#endif
                            a = rotation * a;
                            b = rotation * b;

                            DrawLine(origin + position, dest + position, 6, Color.black);
                            DrawLine(dest + position, dest + position + a * 11f, 6, Color.black);
                            DrawLine(dest + position, dest + position + b * 11f, 6, Color.black);

                            DrawLine(origin + position, dest + position, 4, Color.white);
                            DrawLine(dest + position, dest + position + a * 10f, 4, Color.white);
                            DrawLine(dest + position, dest + position + b * 10f, 4, Color.white);

                            DrawLabel(events[i].functionName, origin + position + new Vector2(0f, 6f));
                        }

                        if (m_GizmoDragHandle[currGizmo * 2 + 0].Draw(position + origin, mouse, MouseCursor.Pan))
                        {
                            var index = i;
                            m_GizmoDragHandle[currGizmo * 2 + 0].action = (Vector2 v) =>
                                {
                                    events[index].intParameter = HitboxManager.EncodeVector2ToInt(v / scale);
                                    AnimationUtility.SetAnimationEvents(animation.clip, events);
                                };
                            m_MouseOverHandle = m_GizmoDragHandle[currGizmo * 2 + 0];
                        }
                        if (m_GizmoDragHandle[currGizmo * 2 + 1].Draw(position + dest, mouse, MouseCursor.Pan))
                        {
                            var index = i;
                            m_GizmoDragHandle[currGizmo * 2 + 1].action = (Vector2 v) =>
                            {
                                events[index].floatParameter = HitboxManager.EncodeVector2ToInt(v / scale);
                                AnimationUtility.SetAnimationEvents(animation.clip, events);
                            };
                            m_MouseOverHandle = m_GizmoDragHandle[currGizmo * 2 + 1];
                        }

                        if (eventType == EventType.MouseDown && m_MouseOverHandle != null && eventButton == 0)
                        {
                            m_ActiveHandle = m_MouseOverHandle;
                            //remove this click from the event so we don't click through other gizmos beneath us.
                            eventButton = 3;
                        }
                        else if (eventType == EventType.MouseUp && eventButton == 0)
                            m_ActiveHandle = null;
                        else if (eventType == EventType.MouseDrag && m_ActiveHandle != null && m_ActiveHandle.action != null)
                            m_ActiveHandle.action(mouse - position);

                        currGizmo++;
                    }
                }
            }

            if (m_PivotHandle.Draw(m_ActiveHandle == m_PivotHandle ? mouse : position, mouse, MouseCursor.Pan)
                && m_ActiveHandle == null)
                m_MouseOverHandle = m_PivotHandle;
            else if (m_MouseOverHandle == m_PivotHandle)
                m_MouseOverHandle = null;

            if (m_ActiveHandle == null && m_MouseOverHandle == m_PivotHandle && eventType == EventType.MouseDown && eventButton == 0)
            {
                m_ActiveHandle = m_MouseOverHandle;
            }
            else if (m_ActiveHandle != null && m_ActiveHandle == m_PivotHandle && eventType == EventType.MouseUp)
            {
                if (m_Renderer == null)
                    m_Renderer = manager.GetComponent<SpriteRenderer>();

                var path = AssetDatabase.GetAssetPath(m_Renderer.sprite);

                TextureImporter import = (TextureImporter)AssetImporter.GetAtPath(path);
                if (import.spriteImportMode == SpriteImportMode.Multiple && import.spritesheet != null)
                {
                    var metadata = import.spritesheet;

                    import.isReadable = true;
                    for (int i = 0; i < metadata.Length; i++)
                        if (metadata[i].name.Equals(m_Renderer.sprite.name))
                        {
                            var p = (mouse - (position - pivot)) / scale;

                            p.x = Mathf.Round(p.x) / metadata[i].rect.width;
                            p.y = 1f - (Mathf.Round(p.y) / metadata[i].rect.height);

                            metadata[i].pivot = p;
                            break;
                        }

                    m_ActiveHandle = m_MouseOverHandle = null;
                    import.spritesheet = metadata;
                    EditorUtility.SetDirty(import);
                    import.SaveAndReimport();
                }
                m_ActiveHandle = m_MouseOverHandle = null;
            }

            var alignedPos = m_ActiveHandle == m_PivotHandle ? mouse - (position - pivot) : pivot;

            DrawLabel("Pivot (" + Mathf.RoundToInt(alignedPos.x / scale) + ", " + Mathf.RoundToInt(alignedPos.y / scale) + ")",
                        (m_ActiveHandle == m_PivotHandle ? mouse : position) + new Vector2(-15f, 6f));

            if (m_EditorMenuOpen == false && eventType == EventType.MouseDown && eventButton == 1)
            {
                if (m_Renderer == null)
                    m_Renderer = manager.GetComponent<SpriteRenderer>();

                var size = m_Renderer.sprite.rect.size;
                var pivotPixels = m_Renderer.sprite.pivot;

                CreateEditGizmoMenu(new Vector2(pivotPixels.x / size.x, pivotPixels.y / size.y), mouse);
            }

            ApplySerializedProperties();
        }

        public void DrawColliderRects(Vector2 position, Vector2 pivot, Vector2 pan, Vector2 mouse, float scale)
        {
            UpdateSerializedObject();

            if (Animation == null || SelectedAnimation >= Animation.Length ||
            Animation[SelectedAnimation].framedata == null || SelectedFrame >= Animation[SelectedAnimation].framedata.Length) return;
            var framedata = Animation[SelectedAnimation].framedata[SelectedFrame];
            var colliderArray = framedata.collider;
            var eventType = Event.current.type;
            var eventButton = Event.current.button;

            m_EditorScale = scale;
            m_PreviewOrigin = position;

            for (int i = 0; i < colliderArray.Length; i++)
            {
                var collider = colliderArray[i];
                var color = HitboxSettings.COLOR(collider.type);

                color.a = 0.75f;
                collider.rect.x *= (int)scale;
                collider.rect.y *= (int)scale;
                collider.rect.width *= (int)scale;
                collider.rect.height *= (int)scale;

                Rect rect = new Rect(collider.rect.x, collider.rect.y, collider.rect.width, collider.rect.height);

                if (m_Renderer != null && m_Renderer.flipX)
                {
                    rect.x *= -1;
                    rect.width *= -1;
                }

                var cRect = new Rect(position.x + (rect.x), position.y + -(rect.y + rect.height), rect.width, rect.height);

                if (i != SelectedCollider)
                {
                    if (m_MouseOverHandle == null && m_ActiveHandle == null && cRect.Contains(mouse))
                    {
                        color.a = 0.5f;

                        if (eventType == EventType.MouseDown)
                        {
                            if (eventButton < 2)
                                SelectedCollider = i;
                            if (eventButton == 1)
                                CreateColliderEditMenu(mouse);
                            else if (eventButton == 0)
                                m_EditorMenuOpen = false;

                            return;
                        }
                    }
                    else
                        color.a = 0.25f;
                }

                if (eventType == EventType.Repaint)
                    EditorGUI.DrawRect(cRect, color);

                //If we are editing this collider we need to display the editor doo dads
                if (SelectedCollider == i)
                {
                    m_MouseOverHandle = null;

                    if (m_ColliderDragHandle[0].Draw(new Vector2(cRect.x, cRect.y), mouse, MouseCursor.ResizeUpLeft))
                        m_MouseOverHandle = m_ColliderDragHandle[0];

                    if (m_ColliderDragHandle[1].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y), mouse, MouseCursor.ResizeVertical))
                        m_MouseOverHandle = m_ColliderDragHandle[1];

                    if (m_ColliderDragHandle[2].Draw(new Vector2(cRect.x + cRect.width, cRect.y), mouse, MouseCursor.ResizeUpRight))
                        m_MouseOverHandle = m_ColliderDragHandle[2];

                    if (m_ColliderDragHandle[3].Draw(new Vector2(cRect.x + cRect.width, cRect.y + cRect.height / 2f), mouse, MouseCursor.ResizeHorizontal))
                        m_MouseOverHandle = m_ColliderDragHandle[3];

                    if (m_ColliderDragHandle[4].Draw(new Vector2(cRect.x + cRect.width, cRect.y + cRect.height), mouse, MouseCursor.ResizeUpLeft))
                        m_MouseOverHandle = m_ColliderDragHandle[4];

                    if (m_ColliderDragHandle[5].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y + cRect.height), mouse, MouseCursor.ResizeVertical))
                        m_MouseOverHandle = m_ColliderDragHandle[5];

                    if (m_ColliderDragHandle[6].Draw(new Vector2(cRect.x, cRect.y + cRect.height), mouse, MouseCursor.ResizeUpRight))
                        m_MouseOverHandle = m_ColliderDragHandle[6];

                    if (m_ColliderDragHandle[7].Draw(new Vector2(cRect.x, cRect.y + cRect.height / 2f), mouse, MouseCursor.ResizeHorizontal))
                        m_MouseOverHandle = m_ColliderDragHandle[7];

                    if (m_ColliderDragHandle[8].Draw(new Vector2(cRect.x + cRect.width / 2f, cRect.y + cRect.height / 2f), mouse, MouseCursor.Pan))
                        m_MouseOverHandle = m_ColliderDragHandle[8];

                    if (eventType == EventType.Repaint)
                    {
                        DrawLabel(collider.type.ToString(), new Vector2(cRect.x + 4, cRect.y + 4));
                        DrawLabel("X: " + Mathf.Round((collider.rect.x + pivot.x) / scale), new Vector2(cRect.x + 4, cRect.y + 22), 10);
                        DrawLabel("Y: " + -Mathf.Round((collider.rect.y - pivot.y + collider.rect.height) / scale), new Vector2(cRect.x + 4, cRect.y + 34), 10);
                        DrawLabel("W: " + collider.rect.width / scale, new Vector2(cRect.x + 4, cRect.y + 46), 10);
                        DrawLabel("H: " + collider.rect.height / scale, new Vector2(cRect.x + 4, cRect.y + 58), 10);
                    }

                    if (eventType == EventType.MouseDown)
                    {
                        if (m_MouseOverHandle != null && eventButton == 0)
                            m_ActiveHandle = m_MouseOverHandle;
                        else if (m_EditorMenuOpen == false && eventButton == 1 && cRect.Contains(mouse))
                            CreateColliderEditMenu(mouse);
                    }
                    else if (eventType == EventType.MouseUp && eventButton == 0)
                    {
                        m_ActiveHandle = null;
                    }
                    else if (eventType == EventType.MouseDrag && m_ActiveHandle != null)
                    {
                        var delta = m_ActiveHandle.GetDragRect(mouse) / scale;

                        colliderArray[SelectedCollider].rect.x += Mathf.RoundToInt(delta.x);
                        colliderArray[SelectedCollider].rect.y += Mathf.RoundToInt(delta.y);
                        colliderArray[SelectedCollider].rect.width += Mathf.RoundToInt(delta.z);
                        colliderArray[SelectedCollider].rect.height += Mathf.RoundToInt(delta.w);
                    }
                }
            }

            if (m_EditorMenuOpen == false && eventType == EventType.MouseDown && eventButton == 1)
                CreateEditMenu(mouse);

            ApplySerializedProperties();
        }

#region Line Drawing
        static private Texture2D m_LineTexture;
        private void DrawLine(Vector2 start, Vector2 end, int width, Color color)
        {
            Vector2 d = end - start;
            float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
            if (d.x < 0)
                a += 180;

            int width2 = (int)Mathf.Ceil(width / 2);

            if (m_LineTexture == null)
            {
                m_LineTexture = new Texture2D(1, 1);
                m_LineTexture.SetPixel(0, 0, Color.white);
                m_LineTexture.Apply();
            }

            var matrix = GUI.matrix;
            GUIUtility.RotateAroundPivot(a, start);
#if UNITY_2017_4_OR_NEWER
            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), m_LineTexture, ScaleMode.StretchToFill, true, 0f, color, 0f, 0f);
#else
            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), m_LineTexture, ScaleMode.StretchToFill, true);
#endif
            GUI.matrix = matrix;
        }
#endregion

#region Menu Functions
        private void ResetMenu()
        {
            m_EditorMenuOpen = true;
            m_EditorMenuItem.Clear();
        }

        private void ResizeMenu(Vector2 mouse)
        {
            var viewWidth = EditorGUIUtility.currentViewWidth;
            var menuWidth = 120f;

            for (int i = 0; i < m_EditorMenuItem.Count; i++)
            {
                if (string.IsNullOrEmpty(m_EditorMenuItem[i].label)) continue;
                menuWidth = Mathf.Max(menuWidth, m_EditorMenuItem[i].label.Length * 7.5f);
            }

            if (mouse.x + menuWidth > viewWidth)
                mouse.x = viewWidth - menuWidth;

            for (int i = 0; i < m_EditorMenuItem.Count; i++)
            {
                m_EditorMenuItem[i].x = mouse.x;
                m_EditorMenuItem[i].width = menuWidth;
            }

            m_EditorMenuRect = new Rect(mouse, new Vector2(menuWidth, 16 * m_EditorMenuItem.Count));
        }

        private void CloseMenu()
        {
            m_EditorMenuOpen = false;
        }

        private void CreateEditGizmoMenu(Vector2 pivot, Vector2 mouse)
        {
            ResetMenu();

            m_EditorMenuItem.Add(new EditorMenuItem("Apply pivot to all frames", 0, (int i) => OnClickApplyPivot(pivot), mouse));

            ResizeMenu(mouse);
        }

        private void CreateEditFrameMenu(float time, Vector2 mouse)
        {
            ResetMenu();

            m_EditorMenuItem.Add(new EditorMenuItem("Add Event", 0, null, mouse, FontStyle.Italic));

            for (int i = 0; i < m_EventLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem(m_EventLabels[i],
                    i + 1,
                    (int index) => OnClickEventLabel(time, index - 1),
                    mouse));

            ResizeMenu(mouse);
        }

        private void CreateColliderEditMenu(Vector2 mouse)
        {
            int i;

            ResetMenu();

            for (i = 0; i < m_TypeLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem("Set: " + m_TypeLabels[i], i, OnClickTypeLabel, mouse));

            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Copy Collider", i++, OnClickCopy, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Delete Collider", i++, OnClickDelete, mouse));
            ResizeMenu(mouse);
        }

        private void CreateEditMenu(Vector2 mouse)
        {
            int i;
            bool showPrev = SelectedFrame > 0 && Animation[SelectedAnimation].framedata[SelectedFrame - 1].collider.Length > 0,
                showNext = SelectedFrame + 1 < Animation[SelectedAnimation].framedata.Length &&
                            Animation[SelectedAnimation].framedata[SelectedFrame + 1].collider.Length > 0;

            ResetMenu();

            for (i = 0; i < m_TypeLabels.Length; i++)
                m_EditorMenuItem.Add(new EditorMenuItem("Create: " + m_TypeLabels[i], i, OnClickCreate, mouse));

            
            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Paste Collider", i++, OnClickPaste, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));
            m_EditorMenuItem.Add(new EditorMenuItem("Copy Frame", i++, OnClickCopyFrame, mouse));
            if (m_FrameClipboard != new Vector2Int(-1, -1)) 
                m_EditorMenuItem.Add(new EditorMenuItem("Paste Frame", i++, OnClickPasteFrame, mouse));

            if (showPrev || showNext)
                m_EditorMenuItem.Add(new EditorMenuItem(null, i++, null, mouse));

            if (showPrev)
                m_EditorMenuItem.Add(new EditorMenuItem("Copy from prev frame", i++, CopyCollidersFromPreviousFrame, mouse));

            if (showNext)
                m_EditorMenuItem.Add(new EditorMenuItem("Copy from next frame", i++, CopyCollidersFromNextFrame, mouse));

            ResizeMenu(mouse);
        }

        private void OnClickTypeLabel(int index)
        {
            var type = (HitboxType)index;
            Animation[SelectedAnimation].framedata[SelectedFrame].collider[SelectedCollider].type = type;
            CloseMenu();
        }

        private void OnClickEventLabel(float time, int eId)
        {
            var e = (FrameEvent)eId;
            var clip = Animation[SelectedAnimation].clip;
            var events = new List<AnimationEvent>(AnimationUtility.GetAnimationEvents(clip));
            var newEvent = new AnimationEvent();

            newEvent.functionName = "EVENT_" + e.ToString();
            newEvent.time = time;
            events.Add(newEvent);

            AnimationUtility.SetAnimationEvents(clip, events.ToArray());

            CloseMenu();
        }

        private void OnClickCreate(int index)
        {
            var list = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame].collider);

            list.Add(new HitboxManager.HitboxColliderData
            {
                type = (HitboxType)index,
                rect = new RectInt(new Vector2Int(Mathf.RoundToInt((m_EditorMenuRect.position.x - m_PreviewOrigin.x) / m_EditorScale),
                                                  Mathf.RoundToInt((m_PreviewOrigin.y - m_EditorMenuRect.position.y) / m_EditorScale - 16)),
                                                  new Vector2Int(16, 16))
            });

            Animation[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            SelectedCollider = list.Count - 1;
            CloseMenu();
        }
#if UNITY_2017_4_OR_NEWER
        Vector2Int m_FrameClipboard = new Vector2Int(-1, -1);
#else
        Vector2 m_FrameClipboard = new Vector2(-1, -1);
#endif


        HitboxManager.HitboxColliderData m_ColliderClipboard = new HitboxManager.HitboxColliderData
        {
            rect = new RectInt(0, 0, 16, 16),
        };

        private void OnClickCopyFrame(int index)
        {
#if UNITY_2017_4_OR_NEWER
            m_FrameClipboard = new Vector2Int(SelectedAnimation, SelectedFrame);
#else
            m_FrameClipboard = new Vector2(SelectedAnimation, SelectedFrame);
#endif
            CloseMenu();
        }

        private void OnClickCopy(int index)
        {
            m_ColliderClipboard = Animation[SelectedAnimation].framedata[SelectedFrame].collider[SelectedCollider];
            CloseMenu();
        }

        private void OnClickPasteFrame(int index)
        {
#if UNITY_2017_4_OR_NEWER
            if (m_FrameClipboard == new Vector2Int(-1, -1)) { CloseMenu();  return; }
            var other = new List<HitboxManager.HitboxColliderData>(Animation[m_FrameClipboard.x].framedata[m_FrameClipboard.y].collider);
#else
            if (m_FrameClipboard == new Vector2(-1, -1)) { CloseMenu(); return; }
            var other = new List<HitboxManager.HitboxColliderData>(Animation[(int)m_FrameClipboard.x].framedata[(int)m_FrameClipboard.y].collider);
#endif
            var list = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame].collider);
            var keep = new List<HitboxManager.HitboxColliderData>(other.Count);

            foreach (var colliderA in other)
            {
                bool hit = false;

                foreach (var colliderB in list)
                    if (colliderA.rect.position == colliderB.rect.position
                        && colliderA.rect.size == colliderB.rect.size
                        && colliderA.type == colliderB.type)
                    {
                        hit = true;
                        break;
                    }

                if (hit == false)
                    keep.Add(colliderA);
            }

            list.AddRange(keep);
            Animation[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }

        private void OnClickPaste(int index)
        {
            var list = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame].collider);

            list.Add(m_ColliderClipboard);
            Animation[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();

            CloseMenu();
        }

        private void OnClickDelete(int index)
        {
            var list = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame].collider);

            list.RemoveAt(SelectedCollider);
            Animation[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }

        private void OnClickApplyPivot(Vector2 pivot)
        {
            if (m_Renderer == null)
                m_Renderer = manager.GetComponent<SpriteRenderer>();

            var path = AssetDatabase.GetAssetPath(m_Renderer.sprite);

            TextureImporter import = (TextureImporter)AssetImporter.GetAtPath(path);
            if (import.spriteImportMode == SpriteImportMode.Multiple && import.spritesheet != null)
            {
                var metadata = import.spritesheet;

                for (int i = 0; i < metadata.Length; i++)
                    metadata[i].pivot = pivot;

                import.spritesheet = metadata;
                EditorUtility.SetDirty(import);
                import.SaveAndReimport();
            }

            CloseMenu();
        }

        private void CopyCollidersFromPreviousFrame(int index)
        {
            CopyCollidersFromFrame(-1);
        }

        private void CopyCollidersFromNextFrame(int index)
        {
            CopyCollidersFromFrame(1);
        }

        private void CopyCollidersFromFrame(int delta)
        {
            var list = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame].collider);
            var other = new List<HitboxManager.HitboxColliderData>(Animation[SelectedAnimation].framedata[SelectedFrame + delta].collider);
            var keep = new List<HitboxManager.HitboxColliderData>(other.Count);

            foreach (var colliderA in other)
            {
                bool hit = false;

                foreach (var colliderB in list)
                    if (colliderA.rect.position == colliderB.rect.position
                        && colliderA.rect.size == colliderB.rect.size
                        && colliderA.type == colliderB.type)
                    {
                        hit = true;
                        break;
                    }

                if (hit == false)
                    keep.Add(colliderA);
            }

            list.AddRange(keep);
            Animation[SelectedAnimation].framedata[SelectedFrame].collider = list.ToArray();
            CloseMenu();
        }
#endregion

#region Label Drawing
        static private GUIStyle m_LabelStyle = new GUIStyle();

        static public void DrawLabel(string text, Vector2 position, int fontSize = 12, FontStyle style = FontStyle.Normal, bool careIfPro = false)
        {
            m_LabelStyle.fontSize = fontSize;
            m_LabelStyle.fontStyle = style;
            if (EditorGUIUtility.isProSkin || careIfPro == false)
            {
                m_LabelStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(position.x, position.y, 128, 16), text, m_LabelStyle);
                m_LabelStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
            else
            {
                m_LabelStyle.normal.textColor = Color.black;
                GUI.Label(new Rect(position.x, position.y - 1f, 128, 16), text, m_LabelStyle);
            }
        }

        static public void DrawEdgeLabel(string text, Vector2 position, Color color, Color edge, int fontSize = 12, FontStyle style = FontStyle.Normal)
        {
            m_LabelStyle.fontSize = fontSize;
            m_LabelStyle.fontStyle = style;
            m_LabelStyle.normal.textColor = edge;
            GUI.Label(new Rect(position.x, position.y - 1, 128, 16), text, m_LabelStyle);
            GUI.Label(new Rect(position.x, position.y + 1, 128, 16), text, m_LabelStyle);
            GUI.Label(new Rect(position.x - 1, position.y, 128, 16), text, m_LabelStyle);
            GUI.Label(new Rect(position.x + 1, position.y, 128, 16), text, m_LabelStyle);
            m_LabelStyle.normal.textColor = color;
            GUI.Label(new Rect(position.x, position.y, 128, 16), text, m_LabelStyle);
        }
        #endregion
        #endregion
    }
}