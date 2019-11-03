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
            { "Tiered_Athena_FloorLoot", "FortInventory" },
            { "Tiered_Athena_FloorLoot_Warmup", "FortInventory" },
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
            //{ "LCD_ToolBox", "" },
            //{ "B_Athena_VendingMachine", "" },
            { "LF_1x1_Foundation", "LF_1x1_Foundation_C"},
            //{ "BuildingFoundation3x75", ""},
            //{ "BuildingFoundation5x74", ""},
            { "BP_CalendarProps", "BP_CalendarProps_C"},
            { "Tiered_Short_Ammo_3_Parent", "Tiered_Short_Ammo_3_Parent_C"},
            // SafeZoneIndicator
            // FortHealthSet

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
