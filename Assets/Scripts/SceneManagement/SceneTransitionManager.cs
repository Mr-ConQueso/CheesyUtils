using UnityEngine;

namespace CheesyUtils.SceneManagement
{
    public class SceneTransitionManager : MonoBehaviour
    {
        // ---- / Public Variables / ---- //
        public bool isFadingIn { get; private set; }
        public bool isFadingOut { get; private set; }
        
        // ---- / Private Variables / ---- //
        private Animator _transitionAnimator;

        private void Awake()
        {
            _transitionAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            EndAnimation();
        }

        public void EndLoadIn()
        {
            isFadingIn = false;
            EndAnimation();
        }
        
        public void EndLoadOut()
        {
            isFadingOut = false;
        }

        public void StartAnimation()
        {
            _transitionAnimator.SetTrigger("triggerStart");
            isFadingIn = true;
        }

        private void EndAnimation()
        {
            _transitionAnimator.SetTrigger("triggerEnd");
            isFadingOut = true;
        }
    }
}