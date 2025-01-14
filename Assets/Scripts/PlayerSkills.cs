using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using App;
using App.Util;
using App.Weapons;
using App.Player;
using DG.Tweening;

public class PlayerSkills : MonoBehaviour
{
    public CharacterModesHandler characterModes;

    [SerializeField] ShotController shotController;

    [SerializeField] AudioSource myAudio;

    [SerializeField] AudioClip charging, explode;

    [SerializeField] GameObject buttonSkillBlock;

    public GunType guntype;

    public bool isSkilling;

    [SerializeField] Animator animPlayer;

    [SerializeField] LayerMask layerMask;

    [SerializeField] float radius;

    [SerializeField] string[] listTag;

    [SerializeField] ParticleSystem particleSystem;

    [SerializeField] float timeParticle;

    [SerializeField] Transform cameraFollow;

    [SerializeField] GameObject UI;


    private void Start()
    {
        // buttonSkillBlock.SetActive(!CallAdsManager.CheckInternet());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ClickSkillHulkSmash();
        }
    }

    public void ClickSkillHulkSmash()
    {
        CallAdsManager.ShowInterstial(CallAdsManager.PositionAds.gameplay_skill_1, () =>
        {
            if (characterModes.GetRunningMode() == CharacterMode.AdvancedFight || characterModes.GetRunningMode() == CharacterMode.Fly ||
                characterModes.GetRunningMode() == CharacterMode.Rope || characterModes.GetRunningMode() == CharacterMode.Climb)
            {
                return;
            }
            myAudio.clip = charging;
            myAudio.Play();
            guntype = shotController.gunType;
            shotController.gunType = GunType.Unknown;
            UI.SetActive(false);
            cameraFollow.DOLocalMove(new Vector3(0, 1.5f, -6.86f), 0.75f);
            cameraFollow.DOLocalRotate(new Vector3(10, 0, 0), 0.75f);
            isSkilling = true;
            animPlayer.Play("Hulk_Smash");
            StartCoroutine(ActiveParticle());
        });
    }

    IEnumerator ActiveParticle()
    {
        yield return new WaitForSeconds(timeParticle);

        myAudio.clip = explode;
        myAudio.Play();
        StartCoroutine(EndSkill());
        particleSystem.Play();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach (var hitCollider in hitColliders)
        {
            if (DoDamageCheck(hitCollider.gameObject))
            {
                if (hitCollider.GetComponent<Health>() != null)
                {
                    hitCollider.GetComponent<Health>().ApplyDamage(200);
                }
                else
                {
                    if (hitCollider.GetComponent<ApplyDamageRedirect>() != null)
                    {
                        if (hitCollider.GetComponent<ApplyDamageRedirect>().targetGO.GetComponent<Health>() != null)
                        {
                            hitCollider.GetComponent<ApplyDamageRedirect>().targetGO.GetComponent<Health>().ApplyDamage(200);
                        }
                    }
                }
            }
        }
    }

    bool DoDamageCheck(GameObject obj)
    {
        for (int i = 0; i < listTag.Length; i++)
        {
            if (obj.CompareTag(listTag[i]))
            {
                return true;
            }
        }
        return false;
    }


    IEnumerator EndSkill()
    {
        yield return new WaitForSeconds(2f);
        cameraFollow.DOLocalMove(Vector3.zero, 0.75f).OnComplete(() =>
        {
            isSkilling = false;

            UI.SetActive(true);
        });
        cameraFollow.DOLocalRotate(Vector3.zero, 0.75f);
        shotController.gunType = guntype;
        buttonSkillBlock.SetActive(!CallAdsManager.CheckInternet());
    }



    public GameObject[] listOld, listNew;

    [ContextMenu("Do It")]
    public void DoIt()
    {
        for (int i = 0; i < listOld.Length; i++)
        {
            var comp0 = listOld[i].GetComponent<Rigidbody>();
            var comp1 = listOld[i].GetComponent<BoxCollider>();
            var comp2 = listOld[i].GetComponent<CapsuleCollider>();
            var comp3 = listOld[i].GetComponent<SphereCollider>();
            var comp4 = listOld[i].GetComponent<CharacterJoint>();
            if (comp0 != null)
            {
                listNew[i].AddComponent<Rigidbody>();
                CopyComponent<Rigidbody>(comp0, listNew[i]);
            }
            if (comp1 != null)
            {
                listNew[i].AddComponent<BoxCollider>();
                CopyComponent<BoxCollider>(comp1, listNew[i]);
            }
            if (comp2 != null)
            {
                listNew[i].AddComponent<CapsuleCollider>();
                CopyComponent<CapsuleCollider>(comp2, listNew[i]);
            }
            if (comp3 != null)
            {
                listNew[i].AddComponent<SphereCollider>();
                CopyComponent<SphereCollider>(comp3, listNew[i]);
            }
            if (comp4 != null)
            {
                listNew[i].AddComponent<CharacterJoint>();
                CopyComponent<CharacterJoint>(comp4, listNew[i]);
            }
        }
    }

    [ContextMenu("Clean")]
    public void DoIt2()
    {
        for (int i = 0; i < listNew.Length; i++)
        {
            var comp0 = listNew[i].GetComponent<Rigidbody>();
            var comp1 = listNew[i].GetComponent<BoxCollider>();
            var comp2 = listNew[i].GetComponent<CapsuleCollider>();
            var comp3 = listNew[i].GetComponent<SphereCollider>();
            var comp4 = listNew[i].GetComponent<CharacterJoint>();
            if (comp0 != null)
            {
                Destroy(listNew[i].GetComponent<Rigidbody>());
                CopyComponent<Rigidbody>(comp0, listNew[i]);
            }
            if (comp1 != null)
            {
                listNew[i].AddComponent<BoxCollider>();
                CopyComponent<BoxCollider>(comp1, listNew[i]);
            }
            if (comp2 != null)
            {
                listNew[i].AddComponent<CapsuleCollider>();
                CopyComponent<CapsuleCollider>(comp2, listNew[i]);
            }
            if (comp3 != null)
            {
                listNew[i].AddComponent<SphereCollider>();
                CopyComponent<SphereCollider>(comp3, listNew[i]);
            }
            if (comp4 != null)
            {
                listNew[i].AddComponent<CharacterJoint>();
                CopyComponent<CharacterJoint>(comp4, listNew[i]);
            }
        }
    }
    

    T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }
}

