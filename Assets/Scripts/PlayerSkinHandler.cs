using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinHandler: MonoBehaviour
{
    public Texture defaultSkin;
    public string skinTextureName;

    private static Transform head;
    private static Transform lArm;
    private static Transform rArm;
    private static Transform lLeg;
    private static Transform rLeg;

    bool animationPlayed;

    private void OnEnable() {
        Texture skinTexture = GetSkinTexture();

        for (int i=0; i<6; i++) {
            Transform c = transform.GetChild(i);

            c.gameObject.GetComponent<Renderer>().material.mainTexture = skinTexture;

            if (c.name.Equals("Head")) { head = c; }
            else if (c.name.Equals("lArm")) { lArm = c; }
            else if (c.name.Equals("rArm")) { rArm = c; }
            else if (c.name.Equals("lLeg")) { lLeg = c; }
            else if (c.name.Equals("rLeg")) { rLeg = c; }


        }
    }

    private Texture GetSkinTexture() {
        Texture skin = Resources.Load($"Textures/{skinTextureName}", typeof(Texture)) as Texture;
        if (skin.Equals(null)) {
            Debug.LogWarning($"Skin not found! ({skinTextureName}) Using default skin!");
            return defaultSkin;
        }
        return skin;
    }

    public static Transform getHead() {
        return head;
    }

    public void WalkAnimation(ref float walkTime, float speed) {
        animationPlayed = true;
        float sinValue = Mathf.Sin(walkTime) * 25;
        walkTime += Time.deltaTime * speed;

        rArm.localEulerAngles = new Vector3(-90f + sinValue, 0f, -90f);
        lArm.localEulerAngles = new Vector3(90f - sinValue, 0f, -90f);

        lLeg.localEulerAngles = new Vector3(90f + sinValue, 0f, -90f);
        rLeg.localEulerAngles = new Vector3(-90f - sinValue, 0f, -90f);

    }

    public void ResetAnimation() {
        if (animationPlayed == false) return;
        rArm.localEulerAngles = new Vector3(-90f, 0, -90f);
        lArm.localEulerAngles = new Vector3(90f, 0, -90f);
        lLeg.localEulerAngles = new Vector3(90f, 0, -90f);
        rLeg.localEulerAngles = new Vector3(-90f, 0, -90f);

        animationPlayed = false;
    }
}
