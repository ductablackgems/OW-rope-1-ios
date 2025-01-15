using UnityEngine;

namespace _0.OW.Scripts.Player
{
    public class OW_PlayerAnimation : MonoBehaviour
    {
        public Animator animator;

        public void PlayIdle()
        {
            animator.Play("Idle");
        }

        public void PlayWalk()
        {
            animator.Play("walk");
        }

        public void PlayRun()
        {
            animator.Play("Run");
        }
    }
}