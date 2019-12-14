using System.Collections.Generic;

namespace Unreal.Core
{
    /// <summary>
    /// Core Redirects enable remapping classes, enums, functions, packages, properties, and structs at load time.
    /// see https://github.com/EpicGames/UnrealEngine/blob/release/Engine/Source/Runtime/CoreUObject/Private/UObject/CoreRedirects.cpp
    /// </summary>
    public static class CoreRedirects
    {
        private static readonly Dictionary<string, string> NativeRedirects = new Dictionary<string, string>
        {
            {"AIDebugComponent","GameplayDebuggingComponent"},
            {"AnimTreeInstance","AnimInstance"},
            {"AnimationCompressionAlgorithm","AnimCompress"},
            {"AnimationCompressionAlgorithm_Automatic","AnimCompress_Automatic"},
            {"AnimationCompressionAlgorithm_BitwiseCompressOnly","AnimCompress_BitwiseCompressOnly"},
            {"AnimationCompressionAlgorithm_LeastDestructive","AnimCompress_LeastDestructive"},
            {"AnimationCompressionAlgorithm_PerTrackCompression","AnimCompress_PerTrackCompression"},
            {"AnimationCompressionAlgorithm_RemoveEverySecondKey","AnimCompress_RemoveEverySecondKey"},
            {"AnimationCompressionAlgorithm_RemoveLinearKeys","AnimCompress_RemoveLinearKeys"},
            {"AnimationCompressionAlgorithm_RemoveTrivialKeys","AnimCompress_RemoveTrivialKeys"},
            {"BlueprintActorBase","Actor"},
            {"DefaultPawnMovement","FloatingPawnMovement"},
            {"DirectionalLightMovable","DirectionalLight"},
            {"DirectionalLightStatic","DirectionalLight"},
            {"DirectionalLightStationary","DirectionalLight"},
            {"DynamicBlockingVolume","BlockingVolume"},
            {"DynamicPhysicsVolume","PhysicsVolume"},
            {"DynamicTriggerVolume","TriggerVolume"},
            {"GameInfo","/Script/Engine.GameMode"},
            {"GameReplicationInfo","/Script/Engine.GameState"},
            {"InterpActor","StaticMeshActor"},
            {"K2Node_CallSuperFunction","/Script/BlueprintGraph.K2Node_CallParentFunction"},
            {"MaterialSpriteComponent","MaterialBillboardComponent"},
            {"MovementComp_Character","CharacterMovementComponent"},
            {"MovementComp_Projectile","ProjectileMovementComponent"},
            {"MovementComp_Rotating","RotatingMovementComponent"},
            {"NavAreaDefault","/Script/NavigationSystem.NavArea_Default"},
            {"NavAreaDefinition","/Script/NavigationSystem.NavArea"},
            {"NavAreaNull","/Script/NavigationSystem.NavArea_Null"},
            {"PhysicsActor","StaticMeshActor"},
            {"PhysicsBSJointActor","PhysicsConstraintActor"},
            {"PhysicsHingeActor","PhysicsConstraintActor"},
            {"PhysicsPrismaticActor","PhysicsConstraintActor"},
            {"PlayerCamera","PlayerCameraManager"},
            {"PlayerReplicationInfo","/Script/Engine.PlayerState"},
            {"PointLightMovable","PointLight"},
            {"PointLightStatic","PointLight"},
            {"PointLightStationary","PointLight"},
            {"RB_BSJointSetup","PhysicsConstraintTemplate"},
            {"RB_BodySetup","BodySetup"},
            {"RB_ConstraintActor","PhysicsConstraintActor"},
            {"RB_ConstraintComponent","PhysicsConstraintComponent"},
            {"RB_ConstraintSetup","PhysicsConstraintTemplate"},
            {"RB_Handle","PhysicsHandleComponent"},
            {"RB_HingeSetup","PhysicsConstraintTemplate"},
            {"RB_PrismaticSetup","PhysicsConstraintTemplate"},
            {"RB_RadialForceComponent","RadialForceComponent"},
            {"RB_SkelJointSetup","PhysicsConstraintTemplate"},
            {"RB_Thruster","PhysicsThruster"},
            {"RB_ThrusterComponent","PhysicsThrusterComponent"},
            {"SensingComponent","PawnSensingComponent"},
            {"SingleAnimSkeletalActor","SkeletalMeshActor"},
            {"SingleAnimSkeletalComponent","SkeletalMeshComponent"},
            {"SkeletalMeshReplicatedComponent","SkeletalMeshComponent"},
            {"SkeletalPhysicsActor","SkeletalMeshActor"},
            {"SoundMode","SoundMix"},
            {"SpotLightMovable","SpotLight"},
            {"SpotLightStatic","SpotLight"},
            {"SpotLightStationary","SpotLight"},
            {"SpriteComponent","BillboardComponent"},
            {"StaticMeshReplicatedComponent","StaticMeshComponent"},
            {"VimBlueprint","AnimBlueprint"},
            {"VimGeneratedClass","AnimBlueprintGeneratedClass"},
            {"VimInstance","AnimInstance"},
            {"WorldInfo","WorldSettings"},
            { "NavAreaMeta", "/Script/NavigationSystem.NavArea_Default"},

            // debugging shootergame
            { "Pickup_AmmoLauncher", "Pickup_AmmoLauncher_C" },
            { "Pickup_AmmoGun", "Pickup_AmmoGun_C" },
            { "Pickup_Health", "Pickup_Health_C" },

            // debugging fortnite
            { "BP_RaceTrack", "BP_RaceTrack" },
            { "CubeAppearEffects", "ActiveGameplayEffect" },
            { "DO_NOT_DELETE_FortWorldManager", "FortWorldManager" },
            { "BP_Athena_Event_Components", "BP_Athena_Event_Components_C" },
            { "BP_ScoreBoard_Place", "BP_ScoreBoard_Place_C" },
            { "Athena_Tree_Pine", "Athena_Tree_Pine_3_C" },
            { "Hedge_Low_Rectangle", "Hedge_Low_Rectangle_C" },
            { "Factory_QuarterWall", "Factory_QuarterWall_C" },
            { "BridgeGirder_Lamp", "BridgeGirder_Lamp_C" },
            { "Prop_Rocks", "Prop_Rocks_04_C" },
            { "BP_Athena_Water", "BP_Athena_Water_C" },
            { "Prop_QuestInteractable_TimeTrials", "Prop_QuestInteractable_TimeTrials_C" },
            { "Tiered_Chest_6_Parent", "Tiered_Chest_6_Parent_C" },
            { "LF_1x1_Foundation", "LF_1x1_Foundation_C"},
            { "BP_CalendarProps", "BP_CalendarProps_C"},
            { "Tiered_Short_Ammo_3_Parent", "Tiered_Short_Ammo_3_Parent_C"},

            //These are mainly props
            /*
            { "BP_IslandScripting", "BP_IslandScripting_C"},
            { "LF_Athena_POI",  "LF_Athena_POI_15x15_C" },
            { "LF_Athena_StreamingTest",  "LF_Athena_POI_15x15_C" },
            { "SM_Athena_DudeBro_RuneGround",  "BP_Athena_RuneSeal_C" },
            { "LCD_RubblePile",  "Athena_Prop_RubblePile_02_C" },
            { "Fence_Chainlink_A01_BarbedWire",  "Fence_Chainlink_A01_BarbedWire_C" },
            { "Prop_TirePile",  "Prop_TirePile_04_C" },
            { "Athena_Prop_ShippingContainer_01_Closed",  "Athena_Prop_ShippingContainer_01_Closed_C" },
            { "Factory_Wall",  "Factory_Wall_3_C" },
            { "Athena_Tree_Large",  "Athena_Tree_Large_1_C" },
            { "SM_Farm_Wall",  "SM_Farm_Wall_2_C" },
            { "Athena_Tree_Medium_03",  "Athena_Tree_Medium_03__C" },
            { "Utility_TrashCanDowntown_01",  "B_DowntownTrashCan_BasketballMinigame_C" },
            { "Car_KCar",  "Car_Stocker_C" },
            { "Street_Light",  "Street_Light__2_C" },
            { "Athena_Tree_Medium_01",  "Athena_Tree_Medium_01__C" },
            { "BuildingFoundation3x",  "BuildingFoundation3x3" },
            { "B_Athena_VendingMachine",  "FortBuildingActorSet" },
            { "SM_Athena_Island_Vortex",  "BGA_DudeBro_Area_Effect_C" },
            { "BuildingFoundation5x",  "BuildingFoundation5x5" },
            { "Athena_SoccerGame",  "Athena_SoccerGame_C" },
            { "Factory_1b_Whole",  "Factory_4c_ThreeQuarter_C" },
            { "Athena_Prop_Storage_Crate01a_side",  "Athena_Prop_Storage_Crate01a_side_C" },
            { "Storage_Metal_Crate",  "Athena_SoccerGame_C" },
            */

        };

        public static string GetRedirect(string path)
        {
            if (NativeRedirects.TryGetValue(path, out string result))
            {
                return result;
            }
            return "";
        }
    }
}
