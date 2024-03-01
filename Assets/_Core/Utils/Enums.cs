namespace _Core.Utils
{
    // Actor:
    public enum DamageCause
    {
        DamagedByActor,
        DamagedByEnvironment,
        DamagedByOther
    }
    
    // Input:
    public enum InputType
    {
        Keyboard,
        Mouse,
        Gamepad
    }
    
    public enum ButtonType
    {
        Button,
        Axis
    }
    
    public enum ButtonState
    {
        Pressed,
        Released,
        Held
    }
}