using UnityEngine.XR;

public class Config : SingletonMonobehaviour<Config>
{
    public TrackingSpaceType trackingSpaceType;

    private void Start() { UpdateConfig(); }

    public void UpdateConfig()
    {
        XRDevice.SetTrackingSpaceType(trackingSpaceType);
    }
}
