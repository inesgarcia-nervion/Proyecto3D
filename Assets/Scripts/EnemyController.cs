using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    private Animator anim;

    [Header("Debug")]
    public bool showDebugRays = true;
    public bool showOnScreenDebug = false;

    [Header("Attack")]
    public float detectionRange = 15f;
    public float attackRange = 2.5f;
    public float attackCooldown = 1.2f;
    public float attackDamage = 1f;
    public float attackHitRadius = 1.0f;
    public LayerMask playerLayer; // asignar la capa del jugador en el inspector

    [Header("Tuning")]
    public float stoppingDistanceOffset = 0.3f;
    public float minForcedAnimSpeed = 0.4f;

    // Estado interno
    bool canAttack = true;
    bool isAttacking = false;

    // Damping para SetFloat speed
    const float speedDampTime = 0.1f;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
            else Debug.LogError("¡No se encuentra el Player! Asegúrate de que tenga el tag 'Player'");
        }

        if (agent == null) agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.speed = 3.5f;
            agent.stoppingDistance = Mathf.Max(0.05f, attackRange - stoppingDistanceOffset);
            agent.updateRotation = true;
            agent.updatePosition = true;
            agent.autoBraking = false;
            agent.acceleration = 12f;
            agent.angularSpeed = 120f;
        }

        // Si no se ha asignado la máscara, intentar usar la capa "Player"
        if (playerLayer == 0)
        {
            int layer = LayerMask.NameToLayer("Player");
            if (layer >= 0) playerLayer = 1 << layer;
            else playerLayer = LayerMask.GetMask("Default");
        }

        // Si hay Rigidbody, evitar conflicto con NavMeshAgent
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Si el agente no está sobre NavMesh, samplear y warp al inicio
        if (agent != null && !agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
                Debug.Log("[EnemyController] Warpeado al NavMesh en Start: " + hit.position);
            }
            else
            {
                Debug.LogWarning("[EnemyController] No se encontró NavMesh cerca en Start. Revisa el bake y layers.");
            }
        }
    }

    void Update()
    {
        if (player == null || agent == null || anim == null) return;

        float distanciaJugador = Vector3.Distance(transform.position, player.position);
        Vector3 direccion = (player.position - transform.position).normalized;

        // 1. Si el jugador está fuera del rango → NO perseguir, NO correr, animación idle/walk = 0
        if (distanciaJugador > detectionRange)
        {
            agent.isStopped = true;
            agent.ResetPath();
            anim.SetFloat("speed", 0f);
            anim.SetBool("isAttacking", false);
            return;
        }

        // 2. Si está dentro del rango → perseguir
        agent.isStopped = false;
        agent.SetDestination(player.position);

        // 3. Determinar velocidad deseada para animación
        float desiredSpeed = agent.desiredVelocity.magnitude;

        // Normalizar para el Animator:
        // 0 = idle, 0.5 = walk, 1 = run
        float animSpeed = 0f;

        if (distanciaJugador > attackRange)
        {
            // CAMINAR si está lejos
            if (distanciaJugador > detectionRange * 0.6f)
            {
                agent.speed = 1.6f; // velocidad caminar
                animSpeed = 0.5f;
            }
            // CORRER si está dentro del rango de persecución
            else
            {
                agent.speed = 3.5f; // velocidad correr
                animSpeed = 1f;
            }

            anim.SetBool("isAttacking", false);
        }
        else
        {
            // 4. En rango de ataque → parar y atacar
            agent.isStopped = true;
            agent.ResetPath();
            animSpeed = 0f;

            if (canAttack && !isAttacking)
                TryAttack();
        }

        // Aplicar suavizado
        float current = anim.GetFloat("speed");
        float smooth = Mathf.Lerp(current, animSpeed, Time.deltaTime * 10f);
        anim.SetFloat("speed", smooth);
    }


    // LateUpdate para sincronizar NavMeshAgent cuando updatePosition está desactivado
    void LateUpdate()
    {
        if (agent != null && !agent.updatePosition)
        {
            // Mantener el NavMeshAgent en la posición del transform para evitar drift
            agent.nextPosition = transform.position;
        }
    }

    // Intento seguro de iniciar ataque: bloquea lógicamente y dispara animación/coroutine
    void TryAttack()
    {
        if (player == null || anim == null) return;

        // Comprobación final de distancia y línea de visión
        float distanciaJugador = Vector3.Distance(transform.position, player.position);
        if (distanciaJugador > attackRange + 0.5f) return;

        Vector3 direccion = (player.position - transform.position).normalized;
        if (Physics.Raycast(transform.position + Vector3.up, direccion, out RaycastHit hit, detectionRange))
        {
            if (hit.collider == null || !hit.collider.CompareTag("Player")) return;

            // Bloqueo lógico
            isAttacking = true;
            canAttack = false;

            // Sincronizar con Animator
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("attack");

            // Iniciar rutina de ataque (movimiento hacia posición de ataque y comprobaciones)
            StartCoroutine(AttackRoutine());
        }
    }

    // Helper: devuelve una posición válida en NavMesh delante del jugador
    Vector3 GetAttackPositionOnNavMesh(Transform playerTransform, float desiredDistanceFromPlayer)
    {
        Vector3 dirToEnemy = (transform.position - playerTransform.position).normalized;
        if (dirToEnemy.sqrMagnitude < 0.001f) dirToEnemy = playerTransform.forward;
        Vector3 target = playerTransform.position + dirToEnemy * desiredDistanceFromPlayer;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        if (NavMesh.SamplePosition(playerTransform.position - playerTransform.forward * desiredDistanceFromPlayer, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    IEnumerator AttackRoutine()
    {
        // NOTA: canAttack e isAttacking ya fueron ajustados en TryAttack()

        if (agent != null && player != null)
        {
            float desiredDistance = Mathf.Clamp(attackRange - 0.1f, 0.3f, attackRange);
            Vector3 attackPos = GetAttackPositionOnNavMesh(player, desiredDistance);

            // Pedimos al agente que vaya a la posición de ataque
            agent.isStopped = false;
            agent.SetDestination(attackPos);

            // Esperar a que llegue (o timeout)
            float timeout = 1.0f;
            float t = 0f;
            while (agent.pathPending && t < timeout) { t += Time.deltaTime; yield return null; }
            while (agent.remainingDistance > agent.stoppingDistance + 0.05f && t < timeout)
            {
                t += Time.deltaTime;
                yield return null;
            }

            // Parar agente y sincronizar
            agent.isStopped = true;
            agent.ResetPath();

            // Si usas root motion: desactivar updatePosition para que la animación mueva al modelo
            bool usingRootMotion = anim.applyRootMotion;
            if (usingRootMotion)
            {
                agent.updatePosition = false;
                agent.updateRotation = false;
            }
            else
            {
                // Asegurar rotación hacia el jugador
                Vector3 lookDir = (player.position - transform.position);
                lookDir.y = 0;
                if (lookDir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }
        else
        {
            // fallback: orientar al jugador si no hay agent
            if (player != null)
            {
                Vector3 lookDir = (player.position - transform.position);
                lookDir.y = 0;
                if (lookDir.sqrMagnitude > 0.001f) transform.rotation = Quaternion.LookRotation(lookDir.normalized);
            }
        }

        // Esperar al frame de impacto (el daño real idealmente se aplica desde Animation Event)
        yield return new WaitForSeconds(0.35f);

        // OverlapSphere para daño como respaldo
        Vector3 center = transform.position + transform.forward * (attackRange * 0.5f) + Vector3.up * 1f;
        Collider[] hits = Physics.OverlapSphere(center, attackHitRadius, playerLayer);
        foreach (var c in hits)
        {
            if (c != null && c.CompareTag("Player"))
            {
                var ph = c.GetComponent<PlayerHealth>();
                if (ph != null)
                {
                    ph.RecibirDaño(attackDamage);
                    if (showDebugRays) Debug.Log("DañarJugador por OverlapSphere aplicado (backup)");
                }
            }
        }

        // Restaurar agente si usamos root motion
        if (agent != null)
        {
            if (anim != null && anim.applyRootMotion)
            {
                agent.updatePosition = true;
                agent.updateRotation = true;
                agent.Warp(transform.position);
            }
            agent.isStopped = false;
        }

        // Esperar cooldown; si no hay Animation Event que llame a OnAttackEnd, usamos este fallback
        yield return new WaitForSeconds(attackCooldown);

        // Si la animación no ha llamado a OnAttackEnd, reseteamos aquí
        if (isAttacking)
        {
            OnAttackEnd();
        }
    }

    // Método público para Animation Event (colocar en el último frame del clip de ataque)
    public void OnAttackEnd()
{
    // Lógica de desbloqueo
    isAttacking = false;
    if (anim != null)
    {
        anim.SetBool("isAttacking", false);
        // Reset trigger por si acaso
        anim.ResetTrigger("attack");
    }

    // Restaurar NavMeshAgent si lo habías desactivado para root motion
    if (agent != null)
    {
        // Si durante el ataque desactivaste updatePosition/updateRotation, reactivarlas
        agent.updatePosition = true;
        agent.updateRotation = true;

        // Reposicionar el agente a la posición actual del transform para evitar drift
        agent.Warp(transform.position);

        // Permitir que el agente vuelva a moverse
        agent.isStopped = false;

        // Forzar un SetDestination al jugador para que agent.desiredVelocity se calcule
        if (player != null)
            agent.SetDestination(player.position);
    }

    // Permitir nuevos ataques
    canAttack = true;

    if (showDebugRays) Debug.Log("[EnemyController] OnAttackEnd: isAttacking=false, agent reactivado");
}


    // Método llamado desde Animation Event en el frame de impacto (si lo usas)
    public void DañarJugador()
    {
        if (showDebugRays) Debug.Log("DañarJugador() llamado desde Animation Event");
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);
        if (distancia <= attackRange + 0.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RecibirDaño(attackDamage);
                if (showDebugRays) Debug.Log("¡Enemigo golpeó al jugador!");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = transform.position + transform.forward * (attackRange * 0.5f) + Vector3.up * 1f;
        Gizmos.DrawWireSphere(center, attackHitRadius);
    }

    void OnGUI()
    {
        if (!showOnScreenDebug || agent == null || player == null) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 220));
        GUILayout.Label($"Enemy Debug:");
        GUILayout.Label($"isOnNavMesh: {agent.isOnNavMesh}");
        GUILayout.Label($"hasPath: {agent.hasPath}");
        GUILayout.Label($"pathStatus: {agent.pathStatus}");
        GUILayout.Label($"pathPending: {agent.pathPending}");
        GUILayout.Label($"desiredVel: {agent.desiredVelocity.magnitude:F2}");
        GUILayout.Label($"vel: {agent.velocity.magnitude:F2}");
        GUILayout.Label($"remaining: {agent.remainingDistance:F2}");
        GUILayout.Label($"stoppingDistance: {agent.stoppingDistance:F2}");
        GUILayout.Label($"distJugador: {Vector3.Distance(transform.position, player.position):F2}");
        GUILayout.Label($"isAttacking: {isAttacking}");
        GUILayout.EndArea();
    }
}
