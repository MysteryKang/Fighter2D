using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AttackType { special = 0, light = 1, kick = 2 }
public class FightingCombo : MonoBehaviour
{
    // list all the keys that compose combo here.
    [Header("Inputs")]
    public KeyCode specialKey;
    public KeyCode lightKey;
    public KeyCode kickKey;


    [Header("InputButtons")]
    public Button lightPunch;
    public Button lightKick;
    public Button jumpButton;
    public Button spButton;
   

    // list all the Attack Animation here
    [Header("Attacks")]
    public Attack specialAttack;
    public Attack lightAttack;
    public Attack kickAttack;
    public List<Combo> combos;
    public float comboLeeway = 0.2f;

    // if input number reaches the number below starts detecting if it is in combo list.
    const int maxComboCount = 3;

    [Header("Components")]
    public Animator ani;

    public Attack curAttack = null;
    ComboInput lastInput = null;
    List<int> currentCombos = new List<int>();
    float timer = 0f;
    public float leeway = 0f;
    bool skip = false;
    bool isComboing = false;

    ComboInput input;

    CharacterController2D controller;
    Player myself;

    float sec = 0;

    bool startCountingTime = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        myself = GetComponent<Player>();
        PrimeCombos();
        SettingUpButtons();
    }

    private void SettingUpButtons() {

        if (lightPunch != null & lightKick != null)
        {
            lightPunch.onClick.AddListener(() =>
            {
                ButtonEvent(AttackType.light);
            });
            lightKick.onClick.AddListener(() =>
            {
                ButtonEvent(AttackType.kick);
            });
            spButton.onClick.AddListener(() =>
            {
                ButtonEvent(AttackType.special);
            });
        }
        else
        {
            return;
        }
    }

    private void ButtonEvent(AttackType type) {
        if (!startCountingTime)
        {
            startCountingTime = true;
        }
        input = new ComboInput(type);
    }

    void PrimeCombos()
    {
        for (int i = 0; i < combos.Count; i++)
        {
            Combo c = combos[i];
            // add listener to those composed combos
            c.onInputted.AddListener(() =>
            {
                // call attack function with the attack
                skip = true;
                Attack(c.comboAttack);
                ResetCombos();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
        lastInput = input;
        if (startCountingTime)
        {
            leeway += Time.deltaTime;
            if (leeway >= comboLeeway)
            {
                startCountingTime = false;
                leeway = 0;
                input = null;
                ResetCombos();
            }
        }
        else {
            leeway = 0;
        }

        if (curAttack != null)
        {
            if (timer >= 0)
                timer -= Time.deltaTime;
            else
                curAttack = null;
            return;
        }

        if (currentCombos.Count > 0)
        {
            leeway += Time.deltaTime;
            if (leeway >= comboLeeway)
            {
                if (lastInput != null)
                {
                    //// Attack(getAttackFromType(lastInput.type));
                    // Debug.Log(lastInput.type);
                    // lastInput = null;
                }
                ResetCombos();
            }
        }
        else
            leeway = 0;

        //ComboInput input = null;

        //#region Getting Inputs 
        //if (Input.GetKeyDown(heavyKey))
        //{
        //    input = new ComboInput(AttackType.heavy);

        //}
        //if (Input.GetKeyDown(lightKey))
        //{
        //    input = new ComboInput(AttackType.light);

        //}

        //if (Input.GetKeyDown(kickKey))
        //{
        //    input = new ComboInput(AttackType.kick);
        //}

        //if (input == null) return;
        //#endregion


        //lastInput = input;

        //List<int> remove = new List<int>();
        //for (int i = 0; i < currentCombos.Count; i++)
        //{
        //    Combo c = combos[currentCombos[i]];
        //    if (c.continueCombo(theInput))
        //        leeway = 0;
        //    else
        //        remove.Add(i);
        //}


        //skip one frame
        //if (skip)
        //{
        //    skip = false;
        //    return;
        //}

        //skip one frame
        //if (skip)
        //{
        //    skip = false;
        //    return;
        //}

        //for (int i = 0; i < combos.Count; i++)
        //{
        //    if (currentCombos.Contains(i)) continue;
        //    if (combos[i].continueCombo(input))
        //    {
        //        currentCombos.Add(i);
        //        leeway = 0;
        //    }
        //}


        if (input != null)
        {
            List<int> remove = new List<int>();
            for (int i = 0; i < currentCombos.Count; i++)
            {
                Combo c = combos[currentCombos[i]];
                if (c.continueCombo(input))
                    leeway = 0;
                else
                    remove.Add(i);
            }

            /// custom button inputs
            for (int i = 0; i < combos.Count; i++)
            {
                if (currentCombos.Contains(i)) continue;
                if (combos[i].continueCombo(input))
                {
                    currentCombos.Add(i);
                    leeway = 0;
                }
            }

            // remove items from the currentCombo list
            foreach (int i in remove)
            {
                currentCombos.RemoveAt(i);
            }
        }
        input = null;
        // this one below is not good for single aka lightattack, cause it has delay time which is comboleeway.
        //if (currentCombos.Count <= 0) // 
        //{
        //    Debug.Log("current combos count == 0");
        //    //  Attack(getAttackFromType(input.type));
        //}

    }

    void ResetCombos()
    {
        leeway = 0;
        for (int i = 0; i < currentCombos.Count; i++)
        {
            Combo c = combos[currentCombos[i]];
            c.ResetCombo();
        }
        currentCombos.Clear();
    }

    Attack getAttackFromType(AttackType t)
    {
        if (t == AttackType.special)
            return specialAttack;
        if (t == AttackType.light)
            return lightAttack;
        if (t == AttackType.kick)
            return kickAttack;
        return null;
    }

    //void SingleAttack(Attack att)
    //{
    //    if (controller.m_Grounded && curAttack == null)
    //    {
    //        //  timer = att.length;
    //        Debug.Log("lightattack");
    //        ani.SetTrigger(att.name);
    //    }
    //}

    void Attack(Attack att)
    {
        if (controller.m_Grounded)
        {
            curAttack = att;
            timer = att.length;
            ani.Play(att.name, -1, 0);
        }
        else
        {
            return;
        }
    }
}


[System.Serializable]
public class Attack
{
    public string name;
    public float length;
}

[System.Serializable]
public class ComboInput
{
    public AttackType type;

    public ComboInput(AttackType t)
    {
        type = t;
    }

    public bool isSameAs(ComboInput t)
    {
        return type == t.type; 
    }
}


[System.Serializable]
public class Combo
{
    public List<ComboInput> inputs;
    public Attack comboAttack;
    public UnityEvent onInputted;
    int curInput = 0;

    public bool continueCombo(ComboInput i)
    {
        if (inputs[curInput].isSameAs(i))
        {
            curInput++;
            if (curInput >= inputs.Count)
            {
                onInputted.Invoke(); // if inputs match combo, invoke the combo action!
                curInput = 0;
            }
            return true;
        }
        else
        {
            curInput = 0;
            return false;
        }
    }

    public ComboInput currentComboInput()
    {
        if (curInput >= inputs.Count) return null;
        return inputs[curInput];
    }

    public void ResetCombo()
    {
        curInput = 0;
    }
}
