using UnityEngine;
using TeamRitual.Core;
using TeamRitual.Character;
using System.Collections.Generic;

namespace BlackGardenStudios.HitboxStudioPro
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class HitboxFeeder : MonoBehaviour
    {
        public ICharacter Owner { get; private set; }
        public BoxCollider2D Collider { get; private set; }
        private HitboxManager m_Manager;

        private int m_hits = 1;
        private float m_Damage = 1f;
        private int m_soundID;
        private bool m_stopSounds;
        private int m_FXUID = 0;
        /**New fields**/
        private int m_hitpause;
        private int m_blockpause;
        private float m_chipDamage;
        private GuardType m_guardType;
        private AttackPriority m_attackPriority;
        private int m_hitstun;
        private int m_blockstun;
        private Vector2 m_hitGroundVelocity;
        private int m_hitGroundVelocityTime;
        private Vector2 m_hitAirVelocity;
        private int m_hitAirVelocityTime;
        private Vector2 m_blockGroundVelocity;
        private int m_blockGroundVelocityTime;
        private Vector2 m_blockAirVelocity;
        private int m_blockAirVelocityTime;

        private float m_giveSelfPower;
        private float m_giveEnemyPower;

        private bool m_downedHit;
        private float m_downedDamage;
        private int m_downedHitstun;
        private Vector2 m_downedVelocity;

        private float m_fallingGravity;
        private bool m_fallAir;
        private bool m_fallGround;
        private bool m_fallRecover;

        private Vector2 m_bounce;
        private float m_bounceGravity;
        private bool m_bounceRecover;
        private float m_slide;
        private int m_slideTime;
        private Vector2 m_wallBounce;
        private float m_wallBounceGravity;
        private float m_wallBounceSlide;
        private int m_wallBounceTime;

        private int m_downTime;
        private bool m_downRecover;

        private int m_hitShakeTime;
        private float m_hitShakeX;
        private float m_hitShakeY;
        private int m_fallShakeTime;
        private float m_minDamage;
        private float m_fallShakeY;

        private bool m_forceStand;
        private bool m_flipEnemy;

        public int m_frame;
        private int m_lastFrame;
        public string m_clipName;
        /****/
        private bool m_DidHit = false;

        public int Id { get; private set; }
        public HitboxType Type { get; private set; }

        void Awake()
        {
            Collider = GetComponent<BoxCollider2D>();
            m_Manager = GetComponentInParent<HitboxManager>();
            Owner = GetComponentInParent<ICharacter>();
            gameObject.tag = transform.parent.tag;

            Collider.enabled = false;
        }

#if UNITY_EDITOR
        private void OnEnable()
        {
            Collider = GetComponent<BoxCollider2D>();
            m_Manager = GetComponentInParent<HitboxManager>();
            Owner = GetComponentInParent<ICharacter>();
        }
#endif
        public void Feed(Vector2 boxSize, Vector2 boxOffset, int ID, HitboxType type,
            float damage, float chipDamage, GuardType guardType, AttackPriority attackPriority, int hitstun, int blockstun,
            Vector2 hitGroundVelocity, int hitGroundVelocityTime, Vector2 hitAirVelocity, int hitAirVelocityTime,
            Vector2 blockGroundVelocity, int blockGroundVelocityTime, Vector2 blockAirVelocity, int blockAirVelocityTime,
            int frame, int hitpause, int blockpause, int fxUID, int hits,
            float giveSelfPower, float giveEnemyPower, bool downedHit, float downedDamage, int downedHitstun, Vector2 downedVelocity,
            float fallingGravity, bool fallAir, bool fallGround, bool fallRecover, Vector2 bounce, float bounceGravity, bool bounceRecover,
            float slide, int slideTime, Vector2 wallBounce, float wallBounceGravity, float wallBounceSlide, int wallBounceTime,
            int downTime, bool downRecover, int hitShakeTime, float hitShakeX, float hitShakeY, int fallShakeTime, float minDamage, float fallShakeY,
            bool forceStand, bool flipEnemy, int soundid, bool stopSounds,
            string animationName
            )
        {
            Type = type;
            m_Damage = damage;
            m_FXUID = fxUID;
            m_soundID = soundid;
            m_stopSounds = stopSounds;
            /**Feed new fields**/
            m_hitpause = hitpause;
            m_blockpause = blockpause;
            m_chipDamage = chipDamage;
            m_guardType = guardType;
            m_attackPriority = attackPriority;
            m_hitstun = hitstun;
            m_blockstun = blockstun;
            m_hitGroundVelocity = hitGroundVelocity;
            m_hitGroundVelocityTime = hitGroundVelocityTime;
            m_hitAirVelocity = hitAirVelocity;
            m_hitAirVelocityTime = hitAirVelocityTime;
            m_blockGroundVelocity = blockGroundVelocity;
            m_blockGroundVelocityTime = blockGroundVelocityTime;
            m_blockAirVelocity = blockAirVelocity;
            m_blockAirVelocityTime = blockAirVelocityTime;
            m_giveSelfPower = giveSelfPower;
            m_giveEnemyPower = giveEnemyPower;
            m_downedHit = downedHit;
            m_downedDamage = downedDamage;
            m_downedHitstun = downedHitstun;
            m_downedVelocity = downedVelocity;
            m_fallingGravity = fallingGravity;
            m_fallAir = fallAir;
            m_fallGround = fallGround;
            m_fallRecover = fallRecover;
            m_bounce = bounce;
            m_bounceGravity = bounceGravity;
            m_bounceRecover = bounceRecover;
            m_slide = slide;
            m_slideTime = slideTime;
            m_wallBounce = wallBounce;
            m_wallBounceGravity = wallBounceGravity;
            m_wallBounceSlide = wallBounceSlide;
            m_wallBounceTime = wallBounceTime;
            m_downTime = downTime;
            m_downRecover = downRecover;
            m_hitShakeTime = hitShakeTime;
            m_hitShakeX = hitShakeX;
            m_hitShakeY = hitShakeY;
            m_fallShakeTime = fallShakeTime;
            m_minDamage = minDamage;
            m_fallShakeY = fallShakeY;
            m_forceStand = forceStand;
            m_flipEnemy = flipEnemy;
            m_lastFrame = m_frame;
            m_frame = frame;
            /****/
            Collider.size = boxSize;
            Collider.offset = boxOffset;
            Collider.isTrigger = true;
            Id = ID;
            m_DidHit = false;
            m_hits = hits;
            m_clipName = animationName;

            Collider.enabled = true;
        }

        public void UpdatePoiseDamage(float damage) {  }
        public void UpdateAttackDamage(float damage) { m_Damage = damage; }

        public void Disable()
        {
            if (Collider != null)
                Collider.enabled = false;
        }

        private bool ReportHit(int target)
        {
            return m_Manager.ReportHit(Id, target);
        }

        private bool PeekHit(int target)
        {
            return m_Manager.PeekReport(Id, target);
        }

        private HitboxFeeder GetFeederFromCollision(Collider2D collision)
        {
            var feeder = collision.GetComponent<HitboxFeeder>();
            if (feeder == null) return null;

            if (collision.GetComponent<HitboxFeeder>() != null) {
                HitboxFeeder colFeeder = collision.GetComponent<HitboxFeeder>();
                if (colFeeder.Type == HitboxType.TRIGGER && this.Type == HitboxType.TRIGGER && 
                    colFeeder.Owner is PlayerGameObj && this.Owner is PlayerGameObj) {
                    CharacterStateMachine enemy = (colFeeder.Owner as PlayerGameObj).stateMachine;
                    CharacterStateMachine stateMachine = (this.Owner as PlayerGameObj).stateMachine;
                    if (enemy.health > 0 && Mathf.Sign(enemy.facing) == -Mathf.Sign(stateMachine.VelX())) {
                        if (enemy.atWall != 0) {
                            if (stateMachine.DistanceToEnemy() < enemy.width/1.7) {
                                if (Mathf.Abs(stateMachine.VelX()) > 0 && Mathf.Sign(stateMachine.VelX()) == enemy.atWall) {
                                    stateMachine.VelXDirect(0);
                                } else if (Mathf.Abs(stateMachine.VelX()) == 0 && Mathf.Abs(enemy.VelX()) == 0) {
                                    stateMachine.VelXDirect(-enemy.atWall*enemy.width*5);
                                }
                            }
                        } else if (stateMachine.atWall == 0) {
                            if (Mathf.Abs(stateMachine.VelX()) > 0 && Mathf.Abs(stateMachine.VelX()) >= Mathf.Abs(enemy.VelX())
                                && stateMachine.currentState.moveType == enemy.currentState.moveType) {
                                enemy.VelXDirect(stateMachine.VelX());
                            } else if (Mathf.Abs(enemy.VelX()) == 0 && stateMachine.DistanceToEnemy() < enemy.width/1.7) {
                                enemy.VelXDirect(stateMachine.facing*enemy.width*5);
                            }
                        }
                    }
                }
            }

            //if this hitbox already hit someone this frame they need to wait a frame.
            if (feeder.m_DidHit == true) return null;
            //Check if pair passes matrix
            var test = HitboxCollisionMatrix.TestPair(Type, feeder.Type);
            //Since both objects will perform a collision test, only invoke an event if we are receiving.
            if (test != HitboxCollisionMatrix.EVENT.RECV && test != HitboxCollisionMatrix.EVENT.BOTH) return null;
            
            return feeder;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            var feeder = GetFeederFromCollision(collision);

            if (feeder != null)
                m_Manager.AddContact(this, feeder);
        }

        /// <summary>
        /// Solve a contact between this feeder and param feeder then fire any applicable events.
        /// </summary>
        public void HandleContact(HitboxFeeder feeder)
        {
            //Lets ask the manager if we should report this.
            if (feeder.ReportHit(m_Manager.UID) == false)
                //Hit wasn't reported, this animation must have already hit us in a previous frame.
                return;

            //Consume the other hurtboxes hit for this frame.
            feeder.m_DidHit = true;

            //Debug.Log("");
            //Debug.Log(feeder.m_hits + " " + feeder.m_Manager.GetCurrentMaxHits());
            //Debug.Log(feeder.m_clipName + " " + feeder.m_Manager.GetCurrentAnimationName());
            
            if (feeder.m_hits != feeder.m_Manager.GetCurrentMaxHits()) {
                return;
            }

            //Debug.Log(m_Manager.UID + ": " + feeder.m_frame + " " + feeder.m_lastFrame);
            
            
            //Proceed to generate contact data and pass event to the owner.
            var collision = feeder.Collider;
            
            //Estimate approximately where the intersection took place.
            var contactPoint = Collider.bounds.ClosestPoint(collision.bounds.center);
            var startY = Mathf.Min(collision.bounds.center.y + collision.bounds.extents.y, Collider.bounds.center.y + (Collider.bounds.extents.y / 2f));
            var endY = Mathf.Max(collision.bounds.center.y - collision.bounds.extents.y, Collider.bounds.center.y - (Collider.bounds.extents.y / 2f));

            contactPoint.y = Mathf.Lerp(startY, endY, Random.Range(0f, 1f));

            //Calculate force, velocity, direction, and damage.
            Owner.HitboxContact(
                new ContactData
                {
                    MyHitbox = this,
                    TheirHitbox = feeder,
                    PlayerIsSource = feeder.Owner is PlayerGameObj,
                    AnimationName = feeder.m_Manager.GetCurrentAnimationName(),
                    AttackHits = feeder.m_hits,
                    Damage = feeder.m_Damage,
                    Point = contactPoint,
                    /**Setting new fields**/
                    ChipDamage = feeder.m_chipDamage,
                    GuardType = feeder.m_guardType,
                    AttackPriority = feeder.m_attackPriority,
                    Hitpause = feeder.m_hitpause,
                    Blockpause = feeder.m_blockpause,
                    Hitstun = feeder.m_hitstun,
                    Blockstun = feeder.m_blockstun,
                    HitGroundVelocity = feeder.m_hitGroundVelocity,
                    HitGroundVelocityTime = feeder.m_hitGroundVelocityTime,
                    HitAirVelocity = feeder.m_hitAirVelocity,
                    HitAirVelocityTime = feeder.m_hitAirVelocityTime,
                    BlockGroundVelocity = feeder.m_blockGroundVelocity,
                    BlockGroundVelocityTime = feeder.m_blockGroundVelocityTime,
                    BlockAirVelocity = feeder.m_blockAirVelocity,
                    BlockAirVelocityTime = feeder.m_blockAirVelocityTime,
                    HitFrame = feeder.m_frame,
                    Frame = feeder.m_frame,
                    GiveSelfPower = feeder.m_giveSelfPower,
                    GiveEnemyPower = feeder.m_giveEnemyPower,
                    DownedHit = feeder.m_downedHit,
                    DownedDamage = feeder.m_downedDamage,
                    DownedHitstun = feeder.m_downedHitstun,
                    DownedVelocity = feeder.m_downedVelocity,
                    FallingGravity = feeder.m_fallingGravity,
                    FallAir = feeder.m_fallAir,
                    FallGround = feeder.m_fallGround,
                    FallRecover = feeder.m_fallRecover,
                    Bounce = feeder.m_bounce,
                    BounceGravity = feeder.m_bounceGravity,
                    BounceRecover = feeder.m_bounceRecover,
                    Slide = feeder.m_slide,
                    SlideTime = feeder.m_slideTime,
                    WallBounce = feeder.m_wallBounce,
                    WallBounceGravity = feeder.m_wallBounceGravity,
                    WallBounceSlide = feeder.m_wallBounceSlide,
                    WallBounceTime = feeder.m_wallBounceTime,
                    DownTime = feeder.m_downTime,
                    DownRecover = feeder.m_downRecover,
                    HitShakeTime = feeder.m_hitShakeTime,
                    HitShakeX = feeder.m_hitShakeX,
                    HitShakeY = feeder.m_hitShakeY,
                    FallShakeTime = feeder.m_fallShakeTime,
                    MinDamage = feeder.m_minDamage,
                    FallShakeY = feeder.m_fallShakeY,
                    ForceStand = feeder.m_forceStand,
                    FlipEnemy = feeder.m_flipEnemy,
                    /****/
                    fxID = feeder.m_FXUID,
                    SoundID = feeder.m_soundID,
                    StopSounds = feeder.m_stopSounds,
                }
            );
        }
    }
}