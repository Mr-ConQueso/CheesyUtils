using UnityEngine;

// usage: attach this script to any gameobject in which you want to render the webcam display
// create a material, use Unlit/Texture as a shader and place it to the gameobject's material placeholder in the mesh renderer component

namespace CheesyUtils
{
    public class WebCamDisplay : MonoBehaviour
    {
        private void Start()
        {
            WebCamTexture webCam = new WebCamTexture
            {
                deviceName = WebCamTexture.devices[0].name
            };
            this.GetComponent<MeshRenderer>().material.mainTexture = webCam;
            webCam.Play();
        }
    }
}

