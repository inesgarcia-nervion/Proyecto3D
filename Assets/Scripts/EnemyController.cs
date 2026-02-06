using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    private Animator anim;

    [Header("Detección")]
    public float detectionRange = 15f;
    public float attackRange = 2.5f;

    [Header("Ataque")]
    public float attackCooldown = 1.2f;
    public float attackDamage = 1f;
    public float attackHitRadius = 1f;
    public LayerMask playerLayer;

    [Header("Patrulla")]
    public float patrolRadius = 10f;
    public float patrolPointDuration = 4f;

    private Vector3 patrolTarget;
    private float patrolTimer = 0f;

    bool canAttack = true;
    bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        agent.updateRotation = true;
        agent.updatePosition = true;
        agent.autoBraking = false;
    }

    void Update()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        // ============================
        // 1. PATRULLA
        // ============================
        if (dist > detectionRange)
        {
            Patrol();
            return;
        }

        // ============================
        // 2. PERSEGUIR
        // ============================
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (dist > attackRange)
        {
            // CAMINAR
            if (dist > detectionRange * 0.6f)
            {
                agent.speed = 1.6f;
                anim.SetFloat("speed", 0.5f);
            }
            // CORRER
            else
            {
                agent.speed = 3.5f;
                anim.SetFloat("speed", 1f);
            }

            anim.SetBool("isAttacking", false);
        }
        else
        {
            // ============================
            // 3. ATAQUE
            // ============================
            agent.isStopped = true;
            anim.SetFloat("speed", 0f);

            if (canAttack && !isAttacking)
                StartAttack();
        }
    }

    // ============================
    // SISTEMA DE PATRULLA
    // ============================
    void Patrol()
    {
        agent.speed = 1.6f;
        anim.SetBool("isAttacking", false);
        anim.SetFloat("speed", 0.5f);

        patrolTimer -= Time.deltaTime;

        if (patrolTimer <= 0f || Vector3.Distance(transform.position, patrolTarget) < 1f)
        {
            patrolTimer = patrolPointDuration;

            Vector3 randomDir = Random.insideUnitSphere * patrolRadius;
            randomDir.y = 0;

            Vector3 target = transform.position + randomDir;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(target, out hit, patrolRadius, NavMesh.AllAreas))
            {
                patrolTarget = hit.position;
                agent.SetDestination(patrolTarget);
            }
        }
    }

    // ============================
    // ATAQUE SIN TRIGGER
    // ============================
    void StartAttack()
    {
        isAttacking = true;
        canAttack = false;

        anim.SetBool("isAttacking", true);

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {

        // Esperar al frame del impacto
        yield return new WaitForSeconds(0.35f);

        Vector3 center = transform.position + transform.forward * 0.6f + Vector3.up * -0.5f;

        Debug.Log("OverlapSphere lanzado en: " + center);

        Collider[] hits = Physics.OverlapSphere(center, 1.8f, playerLayer);

        foreach (var c in hits)
        {
            Debug.Log("Jugador detectado dentro del ataque");

            PlayerHealth ph = c.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                Debug.Log("RecibirDaño() ejecutado correctamente");
                ph.RecibirDaño(attackDamage);
            }
        }


        // Esperar cooldown
        yield return new WaitForSeconds(attackCooldown);

        EndAttack();
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);
        canAttack = true;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * 0.6f + Vector3.up * -0.5f;
        Gizmos.DrawWireSphere(center, 1.8f);
    }


}
