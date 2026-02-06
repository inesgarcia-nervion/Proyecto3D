using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    private Animator anim;

    [Header("Debug")]
    public bool showDebugRays = true;

    void Start()
    {
        anim = GetComponent<Animator>();

        // VALIDACIONES IMPORTANTES
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogError("¡No se encuentra el Player! Asegúrate de que tenga el tag 'Player'");
        }

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
            Debug.LogWarning("NavMeshAgent no asignado, se asignó automáticamente");
        }

        if (anim == null)
        {
            Debug.LogError("¡No hay Animator en este GameObject!");
        }

        // Configurar el NavMeshAgent
        if (agent != null)
        {
            agent.speed = 3.5f; // Debe coincidir con tu Nav Mesh Agent
            agent.stoppingDistance = 2.0f; // Distancia para atacar
        }
    }

    void Update()
    {
        if (player == null || agent == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);
        Vector3 direccion = (player.position - transform.position).normalized;

        // Animación de caminar (según velocidad del agente)
        if (anim != null)
        {
            anim.SetFloat("speed", agent.velocity.magnitude);
        }

        // Debug visual
        if (showDebugRays)
        {
            Debug.DrawRay(transform.position + Vector3.up, direccion * 15f, Color.yellow);
        }

        if (distancia < 15f)
        {
            if (Physics.Raycast(transform.position + Vector3.up, direccion, out RaycastHit hit, 15f))
            {
                if (showDebugRays)
                {
                    Debug.DrawLine(transform.position + Vector3.up, hit.point, Color.red);
                }

                if (hit.collider.CompareTag("Player"))
                {
                    Debug.Log("¡Jugador detectado!");
                    PerseguirJugador();

                    // Si está muy cerca → atacar
                    if (distancia < 2.5f)
                    {
                        Atacar();
                    }
                }
            }
        }
        else
        {
            // Si el jugador está lejos, detener al agente
            if (anim != null)
            {
                anim.SetFloat("speed", 0);
            }
        }
    }

    void PerseguirJugador()
    {
        agent.SetDestination(player.position);
    }

    void Atacar()
    {
        // Para que no siga corriendo mientras ataca
        agent.SetDestination(transform.position);

        // Mirar hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Dispara la animación de ataque
        if (anim != null)
        {
            anim.SetTrigger("attack");
        }
    }

    // Este método se llama desde un Animation Event en la animación de ataque
    public void DañarJugador()
    {
        if (player == null) return;

        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia < 2.5f)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RecibirDaño(1); // Quita 1 corazón (20 damage)
                Debug.Log("¡Enemigo golpeó al jugador!");
            }
        }
    }
}