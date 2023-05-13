﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Runtime.InteropServices;

namespace BlackGardenStudios.HitboxStudioPro
{
#pragma warning disable 0414
    public class HitboxManager : MonoBehaviour
    {
        #region Serialization
        [Serializable]
        public struct internalFrameData
        {
            public internalHitboxData frame;
            public internalOffsetData nextframe;
            public MovementState movementstate;
            public float framerate;
            public float movementspeed;
            public int numhits;
            public bool hasnextframe;

            public internalFrameData(HitboxAnimation source, int frameID)
            {
                frame = source.framedata[frameID];
                if ((frameID + 1) < source.framedata.Length)
                {
                    hasnextframe = true;
                    nextframe = source.framedata[frameID + 1];
                }
                else
                {
                    hasnextframe = false;
                    nextframe = default(internalOffsetData);
                }
                movementstate = source.movementstate;
                movementspeed = source.movementspeed;
                numhits = source.numhits;

                if (source.clip != null)
                    framerate = source.clip.frameRate;
                else
                    framerate = 0f;
            }

#if UNITY_EDITOR
            public string Serialize()
            {
                List<byte> data = new List<byte>();
                data.AddRange(BinaryStructConverter.ToByteArray((serializableData)this));

                if (frame.collider != null && frame.collider.Length > 0)
                    for (int i = 0; i < frame.collider.Length; i++)
                    {
                        data.AddRange(BinaryStructConverter.ToByteArray(frame.collider[i]));
                    }

                return Convert.ToBase64String(data.ToArray());
            }
#endif

            static public internalFrameData Deserialize(string s)
            {
                var data = Convert.FromBase64String(s);
                var serializedResult = BinaryStructConverter.FromByteArray<serializableData>(data);
                var count = serializedResult.numColliders;
                internalFrameData result = serializedResult;

                if (count > 0)
                {
                    int sizeofFrame = Marshal.SizeOf(typeof(serializableData));
                    int sizeofCollider = Marshal.SizeOf(typeof(HitboxColliderData));
                    var colliderArray = new HitboxColliderData[count];

                    for (int i = 0; i < count; i++)
                    {
                        colliderArray[i] = BinaryStructConverter.FromByteArray<HitboxColliderData>(data, sizeofFrame + sizeofCollider * i);
                    }

                    result.frame.collider = colliderArray;
                }

                return result;
            }

            static public implicit operator internalFrameData(serializableData v)
            {
                return new internalFrameData
                {
                    frame = v.frame,
                    framerate = v.framerate,
                    hasnextframe = v.hasnextframe,
                    movementspeed = v.movementspeed,
                    movementstate = v.movementstate,
                    nextframe = v.nextframe,
                    numhits = v.numhits
                };
            }

            public struct serializableData
            {
                public internalOffsetData frame;
                public internalOffsetData nextframe;
                public MovementState movementstate;
                public int numColliders;
                public float framerate;
                public float movementspeed;
                public int numhits;
                public bool hasnextframe;

                public static implicit operator serializableData(internalFrameData v)
                {
                    return new serializableData
                    {
                        frame = v.frame,
                        framerate = v.framerate,
                        hasnextframe = v.hasnextframe,
                        movementspeed = v.movementspeed,
                        movementstate = v.movementstate,
                        nextframe = v.nextframe,
                        numColliders = (v.frame.collider == null ? 0 : v.frame.collider.Length),
                        numhits = v.numhits
                    };
                }
            }
        }

        [Serializable]
        public struct internalHitboxData
        {
            public HitboxColliderData[] collider;
            public Vector2Int capsuleOffset;
            public bool smoothedOffset;

            public float damage;
            public float chipDamage;
            public GuardType guardType;
            public AttackPriority attackPriority;
            public int hitstun;
            public int blockstun;
            public Vector2 hitGroundVelocity;
            public int hitGroundVelocityTime;
            public Vector2 hitAirVelocity;
            public int hitAirVelocityTime;
            public Vector2 blockGroundVelocity;
            public int blockGroundVelocityTime;
            public Vector2 blockAirVelocity;
            public int blockAirVelocityTime;
            public int hitpause;
            public int blockpause;
            public int hitfxuid;
            public int soundid;
            public bool stopSound;

            public float giveSelfPower;
            public float giveEnemyPower;

            public bool downedHit;
            public float downedDamage;
            public int downedHitstun;
            public Vector2 downedVelocity;

            public float fallingGravity;
            public bool fallAir;
            public bool fallGround;
            public bool fallRecover;

            public Vector2 bounce;
            public float bounceGravity;
            public bool bounceRecover;
            public float slide;
            public int slideTime;
            public Vector2 wallBounce;
            public float wallBounceGravity;
            public float wallBounceSlide;
            public int wallBounceTime;

            public int downTime;
            public bool downRecover;

            public int hitShakeTime;
            public float hitShakeX;
            public float hitShakeY;
            public int fallShakeTime;
            public float minDamage;
            public float fallShakeY;

            public bool forceStand;
            public bool flipEnemy;

            public static implicit operator internalHitboxData(HitboxAnimationFrame v)
            {
                return new internalHitboxData
                {
                    collider = v.collider,
                    capsuleOffset = v.capsuleOffset,
                    smoothedOffset = v.smoothedOffset,
                    
                    /**New Fields**/

                    damage = v.damage,
                    chipDamage = v.chipDamage,
                    guardType = v.guardType,
                    attackPriority = v.attackPriority,
                    hitstun = v.hitstun,
                    blockstun = v.blockstun,
                    hitGroundVelocity = v.hitGroundVelocity,
                    hitGroundVelocityTime = v.hitGroundVelocityTime,
                    hitAirVelocity = v.hitAirVelocity,
                    hitAirVelocityTime = v.hitAirVelocityTime,
                    blockGroundVelocity = v.blockGroundVelocity,
                    blockGroundVelocityTime = v.blockGroundVelocityTime,
                    blockAirVelocity = v.blockAirVelocity,
                    blockAirVelocityTime = v.blockAirVelocityTime,
                    hitpause = v.hitpause,
                    blockpause = v.blockpause,
                    hitfxuid = v.hitfxuid,
                    soundid = v.soundid,
                    stopSound = v.stopSound,
                    giveSelfPower = v.giveSelfPower,
                    giveEnemyPower = v.giveEnemyPower,
                    downedHit = v.downedHit,
                    downedDamage = v.downedDamage,
                    downedHitstun = v.downedHitstun,
                    downedVelocity = v.downedVelocity,
                    fallingGravity = v.fallingGravity,
                    fallAir = v.fallAir,
                    fallGround = v.fallGround,
                    fallRecover = v.fallRecover,
                    bounce = v.bounce,
                    bounceGravity = v.bounceGravity,
                    bounceRecover = v.bounceRecover,
                    slide = v.slide,
                    slideTime = v.slideTime,
                    wallBounce = v.wallBounce,
                    wallBounceGravity = v.wallBounceGravity,
                    wallBounceSlide = v.wallBounceSlide,
                    wallBounceTime = v.wallBounceTime,
                    downTime = v.downTime,
                    downRecover = v.downRecover,
                    hitShakeTime = v.hitShakeTime,
                    hitShakeX = v.hitShakeX,
                    hitShakeY = v.hitShakeY,
                    fallShakeTime = v.fallShakeTime,
                    minDamage = v.minDamage,
                    fallShakeY = v.fallShakeY,
                    forceStand = v.forceStand,
                    flipEnemy = v.flipEnemy,

                    /**************/
                };
            }

            public static implicit operator internalHitboxData(internalOffsetData v)
            {
                return new internalHitboxData
                {
                    capsuleOffset = v.capsuleOffset,
                    smoothedOffset = v.smoothedOffset,
                    damage = v.damage,
                    chipDamage = v.chipDamage,
                    guardType = v.guardType,
                    attackPriority = v.attackPriority,
                    hitstun = v.hitstun,
                    blockstun = v.blockstun,
                    hitGroundVelocity = v.hitGroundVelocity,
                    hitGroundVelocityTime = v.hitGroundVelocityTime,
                    hitAirVelocity = v.hitAirVelocity,
                    hitAirVelocityTime = v.hitAirVelocityTime,
                    blockGroundVelocity = v.blockGroundVelocity,
                    blockGroundVelocityTime = v.blockGroundVelocityTime,
                    blockAirVelocity = v.blockAirVelocity,
                    blockAirVelocityTime = v.blockAirVelocityTime,
                    hitpause = v.hitpause,
                    blockpause = v.blockpause,
                    hitfxuid = v.hitfxuid,
                    soundid = v.soundid,
                    stopSound = v.stopSound,
                    giveSelfPower = v.giveSelfPower,
                    giveEnemyPower = v.giveEnemyPower,
                    downedHit = v.downedHit,
                    downedDamage = v.downedDamage,
                    downedHitstun = v.downedHitstun,
                    downedVelocity = v.downedVelocity,
                    fallingGravity = v.fallingGravity,
                    fallAir = v.fallAir,
                    fallGround = v.fallGround,
                    fallRecover = v.fallRecover,
                    bounce = v.bounce,
                    bounceGravity = v.bounceGravity,
                    bounceRecover = v.bounceRecover,
                    slide = v.slide,
                    slideTime = v.slideTime,
                    wallBounce = v.wallBounce,
                    wallBounceGravity = v.wallBounceGravity,
                    wallBounceSlide = v.wallBounceSlide,
                    wallBounceTime = v.wallBounceTime,
                    downTime = v.downTime,
                    downRecover = v.downRecover,
                    hitShakeTime = v.hitShakeTime,
                    hitShakeX = v.hitShakeX,
                    hitShakeY = v.hitShakeY,
                    fallShakeTime = v.fallShakeTime,
                    minDamage = v.minDamage,
                    fallShakeY = v.fallShakeY,
                    forceStand = v.forceStand,
                    flipEnemy = v.flipEnemy,
                };
            }
        }

        [Serializable]
        public struct internalOffsetData
        {
            public Vector2Int capsuleOffset;
            public bool smoothedOffset;

            public float damage;
            public float chipDamage;
            public GuardType guardType;
            public AttackPriority attackPriority;
            public int hitstun;
            public int blockstun;
            public Vector2 hitGroundVelocity;
            public int hitGroundVelocityTime;
            public Vector2 hitAirVelocity;
            public int hitAirVelocityTime;
            public Vector2 blockGroundVelocity;
            public int blockGroundVelocityTime;
            public Vector2 blockAirVelocity;
            public int blockAirVelocityTime;
            public int hitpause;
            public int blockpause;
            public int hitfxuid;
            public int soundid;
            public bool stopSound;

            public float giveSelfPower;
            public float giveEnemyPower;

            public bool downedHit;
            public float downedDamage;
            public int downedHitstun;
            public Vector2 downedVelocity;

            public float fallingGravity;
            public bool fallAir;
            public bool fallGround;
            public bool fallRecover;

            public Vector2 bounce;
            public float bounceGravity;
            public bool bounceRecover;
            public float slide;
            public int slideTime;
            public Vector2 wallBounce;
            public float wallBounceGravity;
            public float wallBounceSlide;
            public int wallBounceTime;

            public int downTime;
            public bool downRecover;

            public int hitShakeTime;
            public float hitShakeX;
            public float hitShakeY;
            public int fallShakeTime;
            public float minDamage;
            public float fallShakeY;

            public bool forceStand;
            public bool flipEnemy;

            public static implicit operator internalOffsetData(HitboxAnimationFrame v)
            {
                return new internalOffsetData
                {
                    capsuleOffset = v.capsuleOffset,
                    smoothedOffset = v.smoothedOffset,
                    damage = v.damage,
                    chipDamage = v.chipDamage,
                    guardType = v.guardType,
                    attackPriority = v.attackPriority,
                    hitstun = v.hitstun,
                    blockstun = v.blockstun,
                    hitGroundVelocity = v.hitGroundVelocity,
                    hitGroundVelocityTime = v.hitGroundVelocityTime,
                    hitAirVelocity = v.hitAirVelocity,
                    hitAirVelocityTime = v.hitAirVelocityTime,
                    blockGroundVelocity = v.blockGroundVelocity,
                    blockGroundVelocityTime = v.blockGroundVelocityTime,
                    blockAirVelocity = v.blockAirVelocity,
                    blockAirVelocityTime = v.blockAirVelocityTime,
                    hitpause = v.hitpause,
                    blockpause = v.blockpause,
                    hitfxuid = v.hitfxuid,
                    soundid = v.soundid,
                    stopSound = v.stopSound,
                    giveSelfPower = v.giveSelfPower,
                    giveEnemyPower = v.giveEnemyPower,
                    downedHit = v.downedHit,
                    downedDamage = v.downedDamage,
                    downedHitstun = v.downedHitstun,
                    downedVelocity = v.downedVelocity,
                    fallingGravity = v.fallingGravity,
                    fallAir = v.fallAir,
                    fallGround = v.fallGround,
                    fallRecover = v.fallRecover,
                    bounce = v.bounce,
                    bounceGravity = v.bounceGravity,
                    bounceRecover = v.bounceRecover,
                    slide = v.slide,
                    slideTime = v.slideTime,
                    wallBounce = v.wallBounce,
                    wallBounceGravity = v.wallBounceGravity,
                    wallBounceSlide = v.wallBounceSlide,
                    wallBounceTime = v.wallBounceTime,
                    downTime = v.downTime,
                    downRecover = v.downRecover,
                    hitShakeTime = v.hitShakeTime,
                    hitShakeX = v.hitShakeX,
                    hitShakeY = v.hitShakeY,
                    fallShakeTime = v.fallShakeTime,
                    minDamage = v.minDamage,
                    fallShakeY = v.fallShakeY,
                    forceStand = v.forceStand,
                    flipEnemy = v.flipEnemy,
                };
            }

            public static implicit operator internalOffsetData(internalHitboxData v)
            {
                return new internalOffsetData
                {
                    capsuleOffset = v.capsuleOffset,
                    smoothedOffset = v.smoothedOffset,
                    damage = v.damage,
                    chipDamage = v.chipDamage,
                    guardType = v.guardType,
                    attackPriority = v.attackPriority,
                    hitstun = v.hitstun,
                    blockstun = v.blockstun,
                    hitGroundVelocity = v.hitGroundVelocity,
                    hitGroundVelocityTime = v.hitGroundVelocityTime,
                    hitAirVelocity = v.hitAirVelocity,
                    hitAirVelocityTime = v.hitAirVelocityTime,
                    blockGroundVelocity = v.blockGroundVelocity,
                    blockGroundVelocityTime = v.blockGroundVelocityTime,
                    blockAirVelocity = v.blockAirVelocity,
                    blockAirVelocityTime = v.blockAirVelocityTime,
                    hitpause = v.hitpause,
                    blockpause = v.blockpause,
                    hitfxuid = v.hitfxuid,
                    soundid = v.soundid,
                    stopSound = v.stopSound,
                    giveSelfPower = v.giveSelfPower,
                    giveEnemyPower = v.giveEnemyPower,
                    downedHit = v.downedHit,
                    downedDamage = v.downedDamage,
                    downedHitstun = v.downedHitstun,
                    downedVelocity = v.downedVelocity,
                    fallingGravity = v.fallingGravity,
                    fallAir = v.fallAir,
                    fallGround = v.fallGround,
                    fallRecover = v.fallRecover,
                    bounce = v.bounce,
                    bounceGravity = v.bounceGravity,
                    bounceRecover = v.bounceRecover,
                    slide = v.slide,
                    slideTime = v.slideTime,
                    wallBounce = v.wallBounce,
                    wallBounceGravity = v.wallBounceGravity,
                    wallBounceSlide = v.wallBounceSlide,
                    wallBounceTime = v.wallBounceTime,
                    downTime = v.downTime,
                    downRecover = v.downRecover,
                    hitShakeTime = v.hitShakeTime,
                    hitShakeX = v.hitShakeX,
                    hitShakeY = v.hitShakeY,
                    fallShakeTime = v.fallShakeTime,
                    minDamage = v.minDamage,
                    fallShakeY = v.fallShakeY,
                    forceStand = v.forceStand,
                    flipEnemy = v.flipEnemy,
                };
            }
        }
        #endregion

        #region Data Types
        [Serializable]
        public struct Matrix2x2
        {
            public Vector2 x;
            public Vector2 y;
        }

        [Serializable]
        public struct HitboxAnimation
        {
            public AnimationClip clip;
            public HitboxAnimationFrame[] framedata;
            public MovementState movementstate;
            public float movementspeed;
            public int numhits;
        }

        [Serializable]
        public struct HitboxAnimationFrame
        {
            public HitboxColliderData[] collider;

            /**New Fields**/

            public float damage;
            public float chipDamage;
            public GuardType guardType;
            public AttackPriority attackPriority;
            public int hitstun;
            public int blockstun;
            public Vector2 hitGroundVelocity;
            public int hitGroundVelocityTime;
            public Vector2 hitAirVelocity;
            public int hitAirVelocityTime;
            public Vector2 blockGroundVelocity;
            public int blockGroundVelocityTime;
            public Vector2 blockAirVelocity;
            public int blockAirVelocityTime;
            public int hitpause;
            public int blockpause;
            public int hitfxlabel;
            public int hitfxuid;
            public int soundfxlabel;
            public int soundid;
            public bool stopSound;
            public float giveSelfPower;
            public float giveEnemyPower;

            public bool downedHit;
            public float downedDamage;
            public int downedHitstun;
            public Vector2 downedVelocity;

            public float fallingGravity;
            public bool fallAir;
            public bool fallGround;
            public bool fallRecover;

            public Vector2 bounce;
            public float bounceGravity;
            public bool bounceRecover;
            public float slide;
            public int slideTime;
            public Vector2 wallBounce;
            public float wallBounceGravity;
            public float wallBounceSlide;
            public int wallBounceTime;

            public int downTime;
            public bool downRecover;

            public int hitShakeTime;
            public float hitShakeX;
            public float hitShakeY;
            public int fallShakeTime;
            public float minDamage;
            public float fallShakeY;

            public bool forceStand;
            public bool flipEnemy;

            /**************/

            public Vector2Int capsuleOffset;
            public float time;
            public bool smoothedOffset;
            public bool skipColliderUpdate;
        }

        [Serializable]
        public struct HitboxColliderData
        {
            public RectInt rect;
            public HitboxType type;
        }

        private struct ContactPair
        {
            public HitboxFeeder a;
            public HitboxFeeder b;
        }
        #endregion

        #region Enums
        [Serializable]
        public enum MovementState
        {
            NO_CHANGE,
            DISABLE_MOVEMENT,
            ENABLE_MOVEMENT,
            DISABLE_DIRECTION_CHANGE,
            ENABLE_DIRECTION_CHANGE,
            ENABLE_BOTH
        }
        #endregion

        #region Properties
        [SerializeField]
        public HitboxAnimation[] m_Animations;
        [SerializeField]
        private int m_MaxHitboxes = 1;

        private SpriteRenderer m_Renderer;
        private List<HitboxFeeder> m_Feeder;
        private List<int> m_RecentHits = new List<int>(16);
        private int lastHitAnim;
        private int lastHitID;
        private int lastHitMaxHits;
        private List<ContactPair> m_Contacts = new List<ContactPair>(10);
        private ICharacter m_Character;
        private float m_UPP = 1f / 32f;
        private int m_UnitsToPixel = 32;
        private Vector2 m_Scale = Vector2.one;
        public int UID { get; private set; }
        static private int m_UIDCounter = 0;
        #endregion

        void Awake()
        {
            UID = m_UIDCounter++;

            m_Character = GetComponent<ICharacter>();

            m_Scale = transform.localScale;
            var feederArray = GetComponentsInChildren<HitboxFeeder>();

            m_Feeder = new List<HitboxFeeder>(m_MaxHitboxes);
            if (feederArray != null)
                m_Feeder.AddRange(feederArray);

            for (int i = m_Feeder.Count; i < m_MaxHitboxes; i++)
                CreateFeeder(i);

            GetSpriteBPP();
        }

        private void OnValidate()
        {
            GetSpriteBPP();
        }

        private void GetSpriteBPP()
        {
            if (m_Renderer == null) m_Renderer = GetComponent<SpriteRenderer>();

            var sprite = m_Renderer.sprite;

            if (sprite != null)
            {
                m_UPP = 1 / m_Renderer.sprite.pixelsPerUnit;
                m_UnitsToPixel = (int)m_Renderer.sprite.pixelsPerUnit;
            }
            else
            {
                Debug.LogWarning("HITBOX MANAGER WARNING: No sprite is assigned during Awake(). Unable to retreive sprite.pixelsPerUnit. Movement and hitbox location calculations will be incorrect!");
            }
        }

        private void CreateFeeder(int index)
        {
            var newGameObject = new GameObject("collider (" + index + ")");
            newGameObject.transform.SetParent(transform, false);
            var collider = newGameObject.AddComponent<BoxCollider2D>();
            var feeder = newGameObject.AddComponent<HitboxFeeder>();

            collider.isTrigger = true;
            collider.enabled = false;
            m_Feeder.Add(feeder);
        }

        #region Animation Baking //Editor Only
#if UNITY_EDITOR
        public void BakeAnimation(int animationID)
        {
            var animation = m_Animations[animationID];

            if (animation.clip == null) return;

            var clip = animation.clip;
            var numFrames = GetNumFrames(animationID);
            var events = AnimationUtility.GetAnimationEvents(clip).Where(
                (AnimationEvent e) => !(e.functionName.Equals("UpdateHitbox") || e.functionName.Equals("_EVENT_ENABLE_MOVE") || e.functionName.Equals("_EVENT_DISABLE_MOVE") || e.functionName.Equals("_EVENT_ENABLE_DIRECTION") || e.functionName.Equals("_EVENT_DISABLE_DIRECTION") || e.functionName.Equals("_EVENT_ENABLE_BOTH") || e.functionName.Equals("_EVENT_SET_SPEED"))
                ).ToList();

            AnimationUtility.SetAnimationEvents(clip, new AnimationEvent[0]);

            for (int k = 0; k < numFrames; k++)
            {
                if (animation.framedata.Length <= k) break;
                var framedata = animation.framedata[k];
                if (framedata.skipColliderUpdate) continue;

                AnimationEvent _event = new AnimationEvent();

                _event.functionName = "UpdateHitbox";
                _event.stringParameter = new internalFrameData(animation, k).Serialize();
                _event.floatParameter = animationID;
                _event.intParameter = k;
                _event.time = framedata.time;
                events.Add(_event);
            }

            if (animation.movementstate != MovementState.NO_CHANGE)
            {
                AnimationEvent move_event = new AnimationEvent();

                switch (animation.movementstate)
                {
                    case MovementState.ENABLE_MOVEMENT:
                        move_event.functionName = "_EVENT_ENABLE_MOVE";
                        break;
                    case MovementState.DISABLE_MOVEMENT:
                        move_event.functionName = "_EVENT_DISABLE_MOVE";
                        break;
                    case MovementState.ENABLE_DIRECTION_CHANGE:
                        move_event.functionName = "_EVENT_ENABLE_DIRECTION";
                        break;
                    case MovementState.DISABLE_DIRECTION_CHANGE:
                        move_event.functionName = "_EVENT_DISABLE_DIRECTION";
                        break;
                    case MovementState.ENABLE_BOTH:
                        move_event.functionName = "_EVENT_ENABLE_BOTH";
                        break;
                }

                if (!string.IsNullOrEmpty(move_event.functionName))
                {
                    move_event.time = 0;
                    events.Add(move_event);
                }
            }

            AnimationEvent speed_event = new AnimationEvent();
            speed_event.functionName = "_EVENT_SET_SPEED";
            speed_event.floatParameter = animation.movementspeed;
            speed_event.time = 0;
            events.Add(speed_event);

            AnimationEvent speed_event2 = new AnimationEvent();
            speed_event2.functionName = "_EVENT_SET_SPEED";
            speed_event2.floatParameter = animation.movementspeed;
            speed_event2.time = animation.clip.length;
            events.Add(speed_event2);

            AnimationUtility.SetAnimationEvents(clip, events.ToArray());
        }

        public void BakeAnimations()
        {
            for (int j = 0; j < m_Animations.Length; j++)
            {
                BakeAnimation(j);
            }
        }
#endif
        #endregion

        private void EVENT_RESET_TARGETS()
        {
            ResetReports();
        }

        private void EVENT_SET_POISE_DAMAGE(float dmg)
        {
            for (int i = 0; i < m_Feeder.Count; i++)
                m_Feeder[i].UpdatePoiseDamage(dmg);
        }

        private void EVENT_SET_ATTACK_DAMAGE(float dmg)
        {
            for (int i = 0; i < m_Feeder.Count; i++)
                m_Feeder[i].UpdateAttackDamage(dmg);
        }

        #region Hidden Events
        private void _EVENT_ENABLE_BOTH()
        {
            SendMessage("EVENT_ENABLE_BOTH", SendMessageOptions.DontRequireReceiver);
        }
        
        private void _EVENT_ENABLE_DIRECTION()
        {
            SendMessage("EVENT_ENABLE_DIRECTION", SendMessageOptions.DontRequireReceiver);
        }

        private void _EVENT_ENABLE_MOVE()
        {
            SendMessage("EVENT_ENABLE_MOVE", SendMessageOptions.DontRequireReceiver);
        }

        private void _EVENT_DISABLE_DIRECTION()
        {
            SendMessage("EVENT_DISABLE_DIRECTION", SendMessageOptions.DontRequireReceiver);
        }

        private void _EVENT_DISABLE_MOVE()
        {
            SendMessage("EVENT_DISABLE_MOVE", SendMessageOptions.DontRequireReceiver);
        }

        private void _EVENT_SET_SPEED(float speed)
        {
            SendMessage("EVENT_SET_SPEED", speed, SendMessageOptions.DontRequireReceiver);
        }
        #endregion

        public string GetCurrentAnimationName() {
            return m_Animations[m_CurrentAnimation].clip.name;
        }

        public int GetCurrentMaxHits() {
            return m_Animations[m_CurrentAnimation].numhits;
        }

        /// <summary>
        /// Record a hit that has been taken, if it has not already been taken.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetid"></param>
        /// <returns>Whether or not the hit has been recorded.</returns>
        public bool ReportHit(int id, int targetid)
        {
            //bool alreadyHit = false;
            var count = m_RecentHits.Count;

            /*if (id == lastHitID && (m_CurrentMaxHitCount != lastHitMaxHits || lastHitAnim != m_CurrentAnimation)) {
                return false;
            }
            lastHitID = id;
            lastHitAnim = m_CurrentAnimation;
            lastHitMaxHits = m_CurrentMaxHitCount;*/

            if (count > 0)
            {
                return true;
                /*if (count >= m_CurrentMaxHitCount)
                {
                    alreadyHit = true;
                }
                else
                {
                    alreadyHit = !m_RecentHits.TryUniqueAdd(targetid);
                }*/
            }
            else
            {
                m_RecentHits.Add(targetid);
                return false;
            }

            //Debug.Log(id + " " +  m_CurrentMaxHitCount + " " + m_Animations[m_CurrentAnimation].clip.name);

            //return !alreadyHit;
        }

        /// <summary>
        /// Check whether or not a hit has been taken without recording it.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="targetid"></param>
        /// <returns>Whether or not the hit was recorded.</returns>
        public bool PeekReport(int id, int targetid)
        {
            return m_RecentHits.Contains(targetid);
        }

        public void ResetReports()
        {
            m_RecentHits.Clear();
        }

        /// <summary>
        /// Add a pair of hitboxes that intersected this frame to the list of pairs to solve.
        /// </summary>
        public void AddContact(HitboxFeeder a, HitboxFeeder b)
        {
            m_Contacts.Add(new ContactPair { a = a, b = b });
        }

        private void LateUpdate()
        {
            m_Contacts.Sort(ContactComparison);

            for (int i = 0; i < m_Contacts.Count; i++)
                m_Contacts[i].a.HandleContact(m_Contacts[i].b);

            m_Contacts.Clear();
        }

        private int ContactComparison(ContactPair x, ContactPair y)
        {
            return ContactDistance(x) - ContactDistance(y);
        }

        private int ContactDistance(ContactPair pair)
        {
            Vector3 aPos = pair.a.transform.position, bPos = pair.b.transform.position;
            float xDirection = Mathf.Sign(bPos.x - aPos.x);

            aPos.x -= xDirection * (pair.a.Collider.size.x / 2f);
            bPos.x += xDirection * (pair.b.Collider.size.x / 2f);

            return Mathf.RoundToInt(Vector3.Distance(aPos, bPos) * 1000f);
        }

        private int m_CurrentMaxHitCount;
        private AnimationClip m_LastClip;
        public int CurrentAnimationUID { get { return m_CurrentId; } }
        private int m_CurrentId;
        private int m_PrivateUID;
        private int m_LastFrame;
        private void UpdateHitbox(AnimationEvent _event)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false) return;
#endif

            if (!string.IsNullOrEmpty(_event.stringParameter))
            {
                Profiler.BeginSample("HITBOXMANAGER: DESERIALIZE DATA");
                var animData = internalFrameData.Deserialize(_event.stringParameter);
                Profiler.EndSample();

                if (m_LastClip != _event.animatorClipInfo.clip)
                {
                    m_LastClip = _event.animatorClipInfo.clip;
                    m_CurrentId = m_PrivateUID++;
                    ResetReports();
                    m_CurrentMaxHitCount = Mathf.Max(1, animData.numhits);
                }

                if (m_LastFrame != _event.intParameter) {
                    ResetReports();
                }
                m_LastFrame = _event.intParameter;

                UpdateHitbox(animData, Mathf.RoundToInt(_event.floatParameter), _event.intParameter);
            }
        }
        
        private Vector2Int m_NextOffset;
        private Vector2 m_OffsetStep;

        private void FixedUpdate()
        {
            if (m_OffsetStep != Vector2.zero)
            {
                var capsule = m_OffsetStep;
                transform.root.localPosition += new Vector3(capsule.x * m_UPP, capsule.y * m_UPP);
            }
        }

        private void UpdateHitbox(internalFrameData animdata, int anim, int frame)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false) return;
#endif
            if (m_Feeder == null) { Debug.Log("WARNING: hitbox feeder for object \"" + gameObject.name + "\" is null or insufficient"); return; }

            m_CurrentAnimation = anim;
            m_CurrentFrame = frame;
            var framedata = animdata.frame;
            var nextframedata = animdata.nextframe;

            Profiler.BeginSample("HITBOXMANAGER: RESET UNUSED COLLIDERS");
            if (framedata.collider != null)
            {
                for (int i = m_Feeder.Count; i < framedata.collider.Length; i++)
                    CreateFeeder(i);

                for (int i = framedata.collider.Length; i < m_Feeder.Count; i++)
                    m_Feeder[i].Disable();
            }
            else
                for (int i = 0; i < m_Feeder.Count; i++)
                    m_Feeder[i].Disable();
            Profiler.EndSample();

            if (animdata.hasnextframe && nextframedata.smoothedOffset)
            {
#if UNITY_EDITOR
                if (!(Application.platform == RuntimePlatform.WindowsEditor && Application.isPlaying == false))
                    transform.root.localPosition += new Vector3(m_OffsetStep.x * m_UPP, m_OffsetStep.y * m_UPP);
#else
            transform.root.localPosition += new Vector3(m_OffsetStep.x * m_UPP, m_OffsetStep.y * m_UPP);
#endif


                m_NextOffset = nextframedata.capsuleOffset;
                if (m_Renderer != null && m_Renderer.flipX)
                    m_NextOffset.x *= -1;

                m_OffsetStep = m_NextOffset;
                m_OffsetStep /= (1f / Time.fixedDeltaTime) / animdata.framerate;
            }
            else
            {
                m_NextOffset = Vector2Int.zero;
                m_OffsetStep = Vector2.zero;
            }
            Profiler.BeginSample("HITBOXMANAGER: FEED COLLIDERS");
            if (framedata.collider != null)
                for (int i = 0; i < framedata.collider.Length; i++)
                {
                    var collider = framedata.collider[i];
                    var rect = collider.rect;

                    if (m_Renderer != null && m_Renderer.flipX)
                    {
                        rect.x *= -1;
                        rect.x -= rect.width;
                    }
                    

                    m_Feeder[i].Feed(new Vector2(rect.width * m_UPP, rect.height * m_UPP),
                        new Vector2(rect.x * m_UPP + (rect.width * m_UPP / 2f), 
                        rect.y * m_UPP + (rect.height * m_UPP / 2f)),
                        m_CurrentId, collider.type,

                        framedata.damage,
                        framedata.chipDamage,
                        framedata.guardType,
                        framedata.attackPriority,
                        framedata.hitstun,
                        framedata.blockstun,
                        framedata.hitGroundVelocity,
                        framedata.hitGroundVelocityTime,
                        framedata.hitAirVelocity,
                        framedata.hitAirVelocityTime,
                        framedata.blockGroundVelocity,
                        framedata.blockGroundVelocityTime,
                        framedata.blockAirVelocity,
                        framedata.blockAirVelocityTime,
                        m_CurrentFrame,
                        framedata.hitpause,
                        framedata.blockpause,
                        framedata.hitfxuid,
                        animdata.numhits,
                        framedata.giveSelfPower,
                        framedata.giveEnemyPower,
                        framedata.downedHit,
                        framedata.downedDamage,
                        framedata.downedHitstun,
                        framedata.downedVelocity,
                        framedata.fallingGravity,
                        framedata.fallAir,
                        framedata.fallGround,
                        framedata.fallRecover,
                        framedata.bounce,
                        framedata.bounceGravity,
                        framedata.bounceRecover,
                        framedata.slide,
                        framedata.slideTime,
                        framedata.wallBounce,
                        framedata.wallBounceGravity,
                        framedata.wallBounceSlide,
                        framedata.wallBounceTime,
                        framedata.downTime,
                        framedata.downRecover,
                        framedata.hitShakeTime,
                        framedata.hitShakeX,
                        framedata.hitShakeY,
                        framedata.fallShakeTime,
                        framedata.minDamage,
                        framedata.fallShakeY,
                        framedata.forceStand,
                        framedata.flipEnemy,
                        framedata.soundid,
                        framedata.stopSound,
                        this.GetCurrentAnimationName()
                        /**************/
                        );
                }
            Profiler.EndSample();
            if (framedata.smoothedOffset == true) return;
#if UNITY_EDITOR
            if (Application.platform == RuntimePlatform.WindowsEditor && Application.isPlaying == false) return;
#endif
            var capsule = framedata.capsuleOffset;
            if (m_Renderer != null && m_Renderer.flipX)
            {
                capsule.x *= -1;
            }

            if (capsule != Vector2Int.zero)
            {
                float x = capsule.x * m_UPP, y = capsule.y * m_UPP;
                transform.root.localPosition += new Vector3(x, y, 0);
            }
        }

        private int m_NumFrames;
        [SerializeField]
        private int m_CurrentAnimation;
        [SerializeField]
        private int m_CurrentFrame;
        [SerializeField]
        public int m_CurrentCollider;

        private void OnEnable()
        {
            m_Scale = transform.localScale;

/*#if UNITY_EDITOR
            if (m_Animations == null || m_Animations.Length == 0) return;
            var animation = m_Animations[m_CurrentAnimation];
            if (animation.clip == null) return;
            m_NumFrames = Mathf.FloorToInt(animation.clip.length * animation.clip.frameRate);
            //if(m_Colliders == null)
            {
                m_Feeder = GetComponentsInChildren<HitboxFeeder>();
                if (m_Feeder == null || m_Feeder.Length < HitboxSettings.MAX_HITBOXES)
                {
                    m_Feeder = new HitboxFeeder[HitboxSettings.MAX_HITBOXES];
                    for (int i = 0; i < m_Feeder.Length; i++)
                    {
                        var newGameObject = new GameObject("collider (" + i + ")");
                        newGameObject.transform.SetParent(transform, false);
                        var collider = newGameObject.AddComponent<BoxCollider2D>();
                        var feeder = newGameObject.AddComponent<HitboxFeeder>();

                        collider.isTrigger = true;
                        collider.enabled = false;
                        m_Feeder[i] = feeder;
                    }
                }
            }
#endif*/
        }

#if UNITY_EDITOR
        public int GetNumFrames(int animationID)
        {
            if (m_Animations == null || animationID >= m_Animations.Length) return 0;
            var animation = m_Animations[animationID];
            if (animation.clip == null) return 0;
            var curves = AnimationUtility.GetObjectReferenceCurveBindings(animation.clip);

            for (int i = 0; i < curves.Length; i++)
            {
                if(curves[i].propertyName.Equals("m_Sprite"))
                {
                    var keyframes = AnimationUtility.GetObjectReferenceCurve(animation.clip, curves[i]);

                    return keyframes.Length;
                }
            }

            Debug.LogWarning("No sprite keyframes have been found in the current animation.");

            return 0;
        }
#endif

        [StructLayout(LayoutKind.Explicit)]
        struct IntConverter
        {
            [FieldOffset(0)]
            public Int32 Value;
            [FieldOffset(0)]
            public Int16 LoValue;
            [FieldOffset(2)]
            public Int16 HiValue;
        }

        static public Vector2 DecodeIntToVector2(int value)
        {
            var data = new IntConverter { Value = value };

            return new Vector2(data.LoValue, data.HiValue);
        }

        static public int EncodeVector2ToInt(Vector2 value)
        {
            return new IntConverter
            {
                LoValue = (short)value.x,
                HiValue = (short)value.y
            }.Value;
        }

        /// <summary>
        /// Decode the data from an animation event to an origin -> direction.
        /// </summary>
        /// <param name="intParam">Int parameter passed by event</param>
        /// <param name="floatParam">Float parameter passed by event</param>
        /// <param name="origin">The local space origin of the gizmo</param>
        /// <param name="direction"> The forward direction of the gizmo</param>
        /// <param name="normalizeDirection"> Whether or not the direction vector output will be normalized</param>
        public void DecodeOriginAndDirection(int intParam, float floatParam, out Vector2 origin, out Vector2 direction, bool normalizeDirection = true)
        {
            origin = DecodeIntToVector2(intParam);
            direction = DecodeIntToVector2((int)floatParam) - origin;
            if (normalizeDirection)
                direction = Vector3.Normalize(direction);
            origin /= m_Renderer.sprite.pixelsPerUnit;
            direction.y *= -1f;
            origin.y *= -1f;
        }

#if UNITY_EDITOR
        public void UpdatePreview()
        {
            if (m_Animations == null || m_Animations.Length == 0 || m_CurrentAnimation >= m_Animations.Length) return;
            var animation = m_Animations[m_CurrentAnimation].clip;
            if (animation == null) return;
            animation.SampleAnimation(gameObject, m_Animations[m_CurrentAnimation].framedata[m_CurrentFrame].time);
        }

        private void OnDrawGizmos()
        {
            if (m_Animations == null || m_CurrentAnimation >= m_Animations.Length ||
                m_Animations[m_CurrentAnimation].framedata == null || m_CurrentFrame >= m_Animations[m_CurrentAnimation].framedata.Length) return;
            var framedata = m_Animations[m_CurrentAnimation].framedata[m_CurrentFrame];
            var collider = framedata.collider;

            for (int i = 0; i < collider.Length; i++)
            {
                var color = HitboxSettings.COLOR(collider[i].type);
                color.a = 0.75f;
                Gizmos.color = color;
                Rect rect = new Rect(collider[i].rect.x * m_UPP, collider[i].rect.y * m_UPP, collider[i].rect.width * m_UPP, collider[i].rect.height * m_UPP);

                if (m_Renderer != null && m_Renderer.flipX)
                {
                    rect.x *= -1;
                    rect.width *= -1;
                }

                Gizmos.DrawCube(new Vector3(transform.position.x + rect.x * m_Scale.x + rect.width / 2f * m_Scale.x, transform.position.y + rect.y * m_Scale.y + rect.height / 2f * m_Scale.y, transform.position.z), new Vector3(rect.width * m_Scale.x, rect.height * m_Scale.y, 1));
            }
        }
#endif
    }

#region Serialization
    public static class BinaryStructConverter
    {
        public static T FromByteArray<T>(byte[] bytes, int offset = 0) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.Copy(bytes, offset, ptr, size);
                object obj = Marshal.PtrToStructure(ptr, typeof(T));
                return (T)obj;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public static byte[] ToByteArray<T>(T obj) where T : struct
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                int size = Marshal.SizeOf(typeof(T));
                ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(obj, ptr, true);
                byte[] bytes = new byte[size];
                Marshal.Copy(ptr, bytes, 0, size);
                return bytes;
            }
            finally
            {
                if (ptr != IntPtr.Zero)
                    Marshal.FreeHGlobal(ptr);
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
#endregion
}

