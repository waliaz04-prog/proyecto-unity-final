using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class Stunnable : MonoBehaviour
{
    private bool isStunned = false;

    [Header("Sonido de aturdimiento")]
    [SerializeField] private string sonidoAturdido;

    public void Stun(float duracion)
    {
        if (isStunned) return;
        StartCoroutine(StunRoutine(duracion));
    }

    private IEnumerator StunRoutine(float duracion)
    {
        isStunned = true;

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Animator ani = GetComponent<Animator>();
        Enemigo1 enemigo1 = GetComponent<Enemigo1>();

        if (!string.IsNullOrEmpty(sonidoAturdido))
            AudioManager.Instance.Play(sonidoAturdido);

        if (agent != null) agent.isStopped = true;
        if (ani != null) ani.enabled = false;
        if (enemigo1 != null) enemigo1.enabled = false;

        yield return new WaitForSeconds(duracion);

        if (agent != null) agent.isStopped = false;
        if (ani != null) ani.enabled = true;
        if (enemigo1 != null) enemigo1.enabled = true;

        isStunned = false;
    }
}
