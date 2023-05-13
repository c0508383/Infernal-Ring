using UnityEngine;

namespace BlackGardenStudios.HitboxStudioPro
{
    public struct ContactData
    {
        public HitboxFeeder MyHitbox;
        public HitboxFeeder TheirHitbox;
        public bool PlayerIsSource;
        public string AnimationName;




        //Set in CharacterStateMachine.Hurt()    vvvv
        public Vector2 HitVelocity;
        public int HitVelocityTime;
        public bool HitFall;
        //                                       ^^^^



        
        /// <summary>
        /// Identifier of the hit effect this attack uses.
        /// </summary>
        public int fxID;

        /// <summary>
        /// Intersection point between these two hitboxes. Place a hit effect at this location.
        /// </summary>
        public Vector2 Point;

        /// <summary>
        /// The number of hits the attack this frame belongs to can make
        /// </summary>
        public int AttackHits;

        /// <summary>
        /// A copy of "Frame" used for detecting hits within the same frame while not erasing frame data.
        /// </summary>
        public int HitFrame;

        /// <summary>
        /// The frame of animation this attack hit at
        /// </summary>
        public int Frame;

        /// <summary>
        /// The type of guard the enemy must make to block this hit.
        /// </summary>
        public GuardType GuardType;
        /// <summary>
        /// The priority/strength of the hit at this frame, compared against other hits.
        /// </summary>
        public AttackPriority AttackPriority;

        /// <summary>
        /// Damage that should be dealt to the enemy
        /// </summary>
        public float Damage;
        /// <summary>
        /// Minimum damage after damage scaling that should be dealt to the enemy
        /// </summary>
        public float MinDamage;
        /// <summary>
        /// Damage that should be dealt to the enemy if they block
        /// </summary>
        public float ChipDamage;

        /// <summary>
        /// The game pauses for this many ticks if the enemy is hit
        /// </summary>
        public int Hitpause;
        /// <summary>
        /// The game pauses for this many ticks if this hit is blocked
        /// </summary>
        public int Blockpause;

        /// <summary>
        /// The amount of time in ticks the enemy is set in a hit state if they block this attack frame
        /// </summary>
        public int Hitstun;

        /// <summary>
        /// If the enemy is on the ground, the enemy is set to this velocity.
        /// </summary>
        public Vector2 HitGroundVelocity;
        /// <summary>
        /// The enemy remains with the hit's ground velocity for this amount of ticks.
        /// </summary>
        public int HitGroundVelocityTime;

        /// <summary>
        /// If the enemy is in the air, the enemy is set to this velocity.
        /// </summary>
        public Vector2 HitAirVelocity;
        /// <summary>
        /// The enemy remains with the hit's air velocity for this amount of ticks.
        /// </summary>
        public int HitAirVelocityTime;

        /// <summary>
        /// The amount of time in ticks the enemy is set in a blocking state if they block this attack frame
        /// </summary>
        public int Blockstun;

        /// <summary>
        /// If the enemy is on the ground, the enemy is set to this velocity.
        /// </summary>
        public Vector2 BlockGroundVelocity;
        /// <summary>
        /// The enemy remains with the hit's ground velocity for this amount of ticks.
        /// </summary>
        public int BlockGroundVelocityTime;

        /// <summary>
        /// If the enemy is in the air, the enemy is set to this velocity.
        /// </summary>
        public Vector2 BlockAirVelocity;
        /// <summary>
        /// The enemy remains with the hit's air velocity for this amount of ticks.
        /// </summary>
        public int BlockAirVelocityTime;

        public float GiveSelfPower;
        public float GiveEnemyPower;

        public bool DownedHit;
        public float DownedDamage;
        public int DownedHitstun;
        public Vector2 DownedVelocity;

        public float FallingGravity;
        public bool FallAir;
        public bool FallGround;
        public bool FallRecover;

        public Vector2 Bounce;
        public float BounceGravity;
        public bool BounceRecover;
        public float Slide;
        public float SlideTime;
        public Vector2 WallBounce;
        public float WallBounceGravity;
        public float WallBounceSlide;
        public int WallBounceTime;

        public int DownTime;
        public bool DownRecover;

        //Not in the GUI yet    vvvvv
        public int HitShakeTime;
        public float HitShakeX;
        public float HitShakeY;
        public int FallShakeTime;
        public float FallShakeY;
        //Not in the GUI yet    ^^^^^

        public bool ForceStand;
        public bool FlipEnemy;

        /// <summary>
        /// Identifier of the sound effect this attack uses.
        /// </summary>
        public int SoundID;
        /// <summary>
        /// If true, all hit sound effects other than this hit's sound effect are stopped
        /// </summary>
        public bool StopSounds;
    }
}