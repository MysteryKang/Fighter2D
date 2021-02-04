using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum AttackType { heavy = 0, light = 1, kick = 2}
public class FightingCombo : MonoBehaviour
{
    // list all the keys that compose combo here.
    [Header("Inputs")]
    public KeyCode heavyKey;
    public KeyCode lightKey;
    public KeyCode kickKey;

    // list all the Attack Animation here
    [Header("Attacks")]
    public Attack heavyAttack;
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
    float leeway = 0f;
    bool skip = false;

    CharacterController2D controller;
    Player myself;

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
            Combo c = combos[i];
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
        if (curAttack != null)
        {
            if (timer >= 0)
                timer -= Time.deltaTime;
            else
                curAttack = null;
            return;
        }


        // if combo input count is bigger than 2
        if (currentCombos.Count > 0)
        {
            leeway += Time.deltaTime;
            if (leeway >= comboLeeway)
            {
                if (lastInput != null && currentCombos.Count >= maxComboCount)
                {
                    Attack(getAttackFromType(lastInput.type));
                    Debug.Log(lastInput.type);
                    lastInput = null;
                }
                ResetCombos();
            }
        }
        else
            leeway = 0;

        ComboInput input = null;
        if (Input.GetKeyDown(heavyKey))
        {
            input = new ComboInput(AttackType.heavy);

        }
        if (Input.GetKeyDown(lightKey))
        {
            input = new ComboInput(AttackType.light);

        }
        if (Input.GetKeyDown(kickKey))
        {
            input = new ComboInput(AttackType.kick);
        }
        if (input == null) return;
        lastInput = input;

        List<int> remove = new List<int>();
        for (int i = 0; i < currentCombos.Count; i++)
        {
            Combo c = combos[currentCombos[i]];
            if (c.continueCombo(input))
                leeway = 0;
            else
                remove.Add(i);
        }

        // skip one frame
        if (skip)
        {
            skip = false;
            return;
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
        if (currentCombos.Count <= 0)
        {
            Attack(getAttackFromType(input.type));
        }
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
        if (t == AttackType.heavy)
            return heavyAttack;
        if (t == AttackType.light)
            return lightAttack;
        if (t == AttackType.kick)
            return kickAttack;
        return null;
    }

    void Attack(Attack att)
    {
        if (controller.m_Grounded)
        {
            curAttack = att;
            timer = att.length;
            ani.Play(att.name, -1, 0);
        }
        else {
            return;
        }
    }
}


[System.Serializable]
public class Attack {
    public string name;
    public float length;
}

[System.Serializable]
public class ComboInput {
    public AttackType type;

    public ComboInput(AttackType t)
    {
        type = t;
    }

    public bool isSameAs(ComboInput t) {
        return (type == t.type); // add && movement == t.movement
    }
}


[System.Serializable]
public class Combo {
    public List<ComboInput> inputs;
    public Attack comboAttack;
    public UnityEvent onInputted;
    int curInput = 0;

    public bool continueCombo(ComboInput i) {
        if (inputs[curInput].isSameAs(i))
        {
            curInput++;
            if (curInput >= inputs.Count)
            {
                onInputted.Invoke();
                curInput = 0;
            }
            return true;
        }
        else {
            curInput = 0;
            return false;
        }
    }

    public ComboInput currentComboInput()
    {
        if (curInput >= inputs.Count) return null;
        return inputs[curInput];
    }

    public void ResetCombo() {
        curInput = 0;
    }
}
