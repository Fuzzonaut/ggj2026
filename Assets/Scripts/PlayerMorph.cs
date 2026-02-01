using UnityEngine;

public class PlayerMorph : MonoBehaviour
{
    [Header("References")]
    public InsanityManager insanityManager;
    public Animator anim;

    [Header("Controllers")]
    public RuntimeAnimatorController phase1Controller; // Original (Normal)
    public RuntimeAnimatorController phase2Controller; // Phase 2 (Override)
    public RuntimeAnimatorController phase3Controller; // Phase 3 (Override)

    private int currentPhase = 1;

    void Start()
    {
        // Ensure we start with Phase 1
        if (anim == null) anim = GetComponent<Animator>();
        anim.runtimeAnimatorController = phase1Controller;
    }

    void Update()
    {
        if (insanityManager == null) return;

        float ins = insanityManager.insanity;

        // Phase 1 Logic (< 33)
        if (ins < 33 && currentPhase != 1)
        {
            ChangePhase(1, phase1Controller);
        }
        // Phase 2 Logic (33 - 66)
        else if (ins >= 33 && ins < 66 && currentPhase != 2)
        {
            ChangePhase(2, phase2Controller);
        }
        // Phase 3 Logic (>= 66)
        else if (ins >= 66 && currentPhase != 3)
        {
            ChangePhase(3, phase3Controller);
        }
    }

    void ChangePhase(int phaseNum, RuntimeAnimatorController newController)
    {
        currentPhase = phaseNum;
        
        // Swap the brain of the animator
        // Note: This might reset the animation to the start frame briefly
        anim.runtimeAnimatorController = newController;
        
        Debug.Log("Morphed to Phase " + phaseNum);
    }
}