using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float distancia = Vector3.Distance(transform.position, player.position);
        Vector3 direccion = (player.position - transform.position).normalized;

        // Animación de caminar (según velocidad del agente)
        anim.SetFloat("speed", agent.velocity.magnitude);

        if (distancia < 15f)
        {
            if (Physics.Raycast(transform.position, direccion, out RaycastHit hit, 15f))
            {
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
    }

    void PerseguirJugador()
    {
        agent.SetDestination(player.position);
    }

    void Atacar()
    {
        // Para que no siga corriendo mientras ataca
        agent.SetDestination(transform.position);

        // Dispara la animación de ataque
        anim.SetTrigger("attack");
    }
}
