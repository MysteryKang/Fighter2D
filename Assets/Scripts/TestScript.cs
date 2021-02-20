using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum AttackType1 { heavy = 0, light = 1, kick = 2 }
public class TestScript : MonoBehaviour
{
    // list all the keys that compose combo here.
    [Header("Inputs")]
    public KeyCode heavyKey;
    public KeyCode lightKey;
    public KeyCode kickKey;

    // list all the Attack Animation here
    [Header("Attacks")]
    public Attack1 heavyAttack;
    public Attack1 lightAttack;
    public Attack1 kickAttack;
    public List<Combo1> combos;
    public float comboLeeway = 0.2f;

    // if input number reaches the number below starts detecting if it is in combo list.
    //const int maxComboCount = 3;

    [Header("Components")]
    public Animator ani;

    public Attack1 curAttack = null;
    ComboInput1 lastInput = null;
    List<int> currentCombos = new List<int>();
    float timer = 0f;
    public float leeway = 0f;
    bool skip = false;
    bool isComboing = false;

    bool lp = false;
    bool lk = false;


    ComboInput1 theInput;

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
    }

    void PrimeCombos()
    {
        for (int i = 0; i < combos.Count; i++)
        {
            Combo1 c = combos[i];
            // add listener to those composed combos
            c.onInputted.AddListener(() =>
            {
                // call attack function with the attack
                Attack(c.comboAttack);
                ResetCombos();
            });
        }
    }

    // Update is called once per frame
    void Update()
    {

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

        ComboInput1 input = null;

        #region Getting Inputs 
        if (Input.GetKeyDown(heavyKey))
        {
            input = new ComboInput1(AttackType1.heavy);

        }
        if (Input.GetKeyDown(lightKey))
        {
            input = new ComboInput1(AttackType1.light);

        }

        if (Input.GetKeyDown(kickKey))
        {
            input = new ComboInput1(AttackType1.kick);
        }

        if (input == null) return;
        #endregion

        if (input != null)
        {
            // SingleAttack(getAttackFromType(input.type));
        }

        lastInput = input;

        List<int> remove = new List<int>();
        for (int i = 0; i < currentCombos.Count; i++)
        {
            Combo1 c = combos[currentCombos[i]];
            if (c.continueCombo(input))
                leeway = 0;
            else
                remove.Add(i);
        }


        for (int i = 0; i < combos.Count; i++)
        {
            if (currentCombos.Contains(i)) continue;
            if (combos[i].continueCombo(input))
            {
                currentCombos.Add(i);
                leeway = 0;

            }
        }


        foreach (int i in remove)
        {
            currentCombos.RemoveAt(i);
        }

       

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
            Combo1 c = combos[currentCombos[i]];
            c.ResetCombo();
        }
        currentCombos.Clear();
    }

    Attack1 getAttackFromType(AttackType1 t)
    {
        if (t == AttackType1.heavy)
            return heavyAttack;
        if (t == AttackType1.light)
            return lightAttack;
        if (t == AttackType1.kick)
            return kickAttack;
        return null;
    }

    void SingleAttack(Attack att)
    {
        if (controller.m_Grounded && curAttack == null)
        {
            //  timer = att.length;
            Debug.Log("lightattack");
            ani.SetTrigger(att.name);
        }
    }

    void Attack(Attack1 att)
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
public class Attack1
{
    public string name;
    public float length;
}

[System.Serializable]
public class ComboInput1
{
    public AttackType1 type;

    public ComboInput1(AttackType1 t)
    {
        type = t;
    }

    public bool isSameAs(ComboInput1 t)
    {
        return (type == t.type);
    }
}


[System.Serializable]
public class Combo1
{
    public List<ComboInput1> inputs;
    public Attack1 comboAttack;
    public UnityEvent onInputted;
    int curInput = 0;

    public bool continueCombo(ComboInput1 i)
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

    public ComboInput1 currentComboInput()
    {
        if (curInput >= inputs.Count) return null;
        return inputs[curInput];
    }

    public void ResetCombo()
    {
        curInput = 0;
    }
}
