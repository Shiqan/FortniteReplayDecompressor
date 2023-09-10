namespace Unreal.Core.Models.Enums;

/// <summary>
/// hard coded names in "UnrealNames.inl"
/// https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Public/UObject/NameTypes.h#L599
/// https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.inl
/// https://github.com/EpicGames/UnrealEngine/blob/70bc980c6361d9a7d23f6d23ffe322a2d6ef16fb/Engine/Source/Runtime/Core/Public/UObject/UnrealNames.h#L31
/// https://github.com/EpicGames/UnrealEngine/blob/375ba9730e72bf85b383c07a5e4a7ba98774bcb9/Engine/Source/Runtime/Core/Private/UObject/UnrealNames.cpp#L283
/// </summary>
public enum UnrealNames
{
    None = 0,
    ByteProperty = 1,
    IntProperty = 2,
    BoolProperty = 3,
    FloatProperty = 4,
    ObjectProperty = 5, // ClassProperty shares the same tag 
    NameProperty = 6,
    DelegateProperty = 7,
    DoubleProperty = 8,
    ArrayProperty = 9,
    StructProperty = 10,
    VectorProperty = 11,
    RotatorProperty = 12,
    StrProperty = 13,
    TextProperty = 14,
    InterfaceProperty = 15,
    MulticastDelegateProperty = 16,
    //Available = 17
    LazyObjectProperty = 18,
    SoftObjectProperty = 19, // SoftClassProperty shares the same tag
    UInt64Property = 20,
    UInt32Property = 21,
    UInt16Property = 22,
    Int64Property = 23,
    Int16Property = 25,
    Int8Property = 26,
    //Available = 27
    MapProperty = 28,
    SetProperty = 29,

    // Special packages.
    Core = 30,
    Engine = 31,
    Editor = 32,
    CoreUObject = 33,

    // More class properties
    EnumProperty = 34,

    // Special types.
    Cylinder = 50,
    BoxSphereBounds = 51,
    Sphere = 52,
    Box = 53,
    Vector2D = 54,
    IntRect = 55,
    IntPoint = 56,
    Vector4 = 57,
    Name = 58,
    Vector = 59,
    Rotator = 60,
    SHVector = 61,
    Color = 62,
    Plane = 63,
    Matrix = 64,
    LinearColor = 65,
    AdvanceFrame = 66,
    Pointer = 67,
    Double = 68,
    Quat = 69,
    Self = 70,
    Transform = 71,

    // Object class names.
    Object = 100,
    Camera = 101,
    Actor = 102,
    ObjectRedirector = 103,
    ObjectArchetype = 104,
    Class = 105,
    ScriptStruct = 106,
    Function = 107,

    // Misc.
    State = 200,
    TRUE = 201,
    FALSE = 202,
    Enum = 203,
    Default = 204,
    Skip = 205,
    Input = 206,
    Package = 207,
    Groups = 208,
    Interface = 209,
    Components = 210,
    Global = 211,
    Super = 212,
    Outer = 213,
    Map = 214,
    Role = 215,
    RemoteRole = 216,
    PersistentLevel = 217,
    TheWorld = 218,
    PackageMetaData = 219,
    InitialState = 220,
    Game = 221,
    SelectionColor = 222,
    UI = 223,
    ExecuteUbergraph = 224,
    DeviceID = 225,
    RootStat = 226,
    MoveActor = 227,
    All = 230,
    MeshEmitterVertexColor = 231,
    TextureOffsetParameter = 232,
    TextureScaleParameter = 233,
    ImpactVel = 234,
    SlideVel = 235,
    TextureOffset1Parameter = 236,
    MeshEmitterDynamicParameter = 237,
    ExpressionInput = 238,
    Untitled = 239,
    Timer = 240,
    Team = 241,
    Low = 242,
    High = 243,
    NetworkGUID = 244,
    GameThread = 245,
    RenderThread = 246,
    OtherChildren = 247,
    Location = 248,
    Rotation = 249,
    BSP = 250,
    EditorSettings = 251,
    AudioThread = 252,
    ID = 253,
    UserDefinedEnum = 254,
    Control = 255,
    Voice = 256,
    Zlib = 257,
    Gzip = 258,

    // Online
    DGram = 280,
    Stream = 281,
    GameNetDriver = 282,
    PendingNetDriver = 283,
    BeaconNetDriver = 284,
    FlushNetDormancy = 285,
    DemoNetDriver = 286,
    GameSession = 287,
    PartySession = 288,
    GamePort = 289,
    BeaconPort = 290,
    MeshPort = 291,
    MeshNetDriver = 292,
    LiveStreamVoice = 293,

    // Texture settings.
    Linear = 300,
    Point = 301,
    Aniso = 302,
    LightMapResolution = 303,

    // Sound.
    //310 = 
    UnGrouped = 311,
    VoiceChat = 312,

    // Optimized replication.
    Playing = 320,
    Spectating = 322,
    Inactive = 325,

    // Log messages.
    PerfWarning = 350,
    Info = 351,
    Init = 352,
    Exit = 353,
    Cmd = 354,
    Warning = 355,
    Error = 356,

    // File format backwards-compatibility.
    FontCharacter = 400,
    InitChild2StartBone = 401,
    SoundCueLocalized = 402,
    SoundCue = 403,
    RawDistributionFloat = 404,
    RawDistributionVector = 405,
    InterpCurveFloat = 406,
    InterpCurveVector2D = 407,
    InterpCurveVector = 408,
    InterpCurveTwoVectors = 409,
    InterpCurveQuat = 410,

    AI = 450,
    NavMesh = 451,

    PerformanceCapture = 500,

    // Special config names - not required to be consistent for network replication
    EditorLayout = 600,
    EditorKeyBindings = 601,
    GameUserSettings = 602,
}
