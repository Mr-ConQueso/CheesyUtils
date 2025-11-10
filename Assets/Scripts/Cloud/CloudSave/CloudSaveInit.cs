using System;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;

namespace CheesyUtils.Cloud
{
    public class CloudSaveInit : MonoBehaviour
    {
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
                TestSave();
            }
            catch (RequestFailedException e)
            {
                Debug.LogError($"Failed to initialize Unity Services: {e.Message}");
                // Handle specific Unity Services authentication errors
                switch (e.ErrorCode)
                {
                    case CommonErrorCodes.InvalidToken:
                        Debug.LogError("Authentication token is invalid");
                        break;
                    case CommonErrorCodes.Timeout:
                        Debug.LogError("Network error - please check your internet connection");
                        break;
                    default:
                        Debug.LogError($"Authentication error {e.ErrorCode}: {e.Message}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error during initialization: {e.Message}");
            }
        }

        private static async void TestSave()
        {
            try
            {
                var data = new Dictionary<string, object>{ { "MySaveKey", "Hello World" } };
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError($"Cloud Save error: {e.Message}");
                // Handle specific Cloud Save errors
                switch (e.ErrorCode)
                {
                    case CommonErrorCodes.RequestRejected:
                        Debug.LogError("Save failed: Request rejected by Cloud Save service");
                        break;
                    default:
                        Debug.LogError($"Cloud Save error {e.ErrorCode}: {e.Message}");
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error during cloud save: {e.Message}");
            }
        }
    }
}