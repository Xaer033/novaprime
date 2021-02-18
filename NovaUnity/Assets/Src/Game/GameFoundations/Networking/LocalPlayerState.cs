using Mirage;

public class LocalPlayerState
{
    public NetPlayer netPlayer;
    public INetworkConnection conn;
    public IInputGenerator pInput;
    public PlayerState state;
    public IAvatarController controller;

}
