using UnityEngine;
using TMPro; 

public class FlagBabi : MonoBehaviour
{
    public GameObject winUI; //panel que sale de la pregunta
    public int idBandera; 
    public string preguntaTexto; 
    

    public TextMeshProUGUI tituloUI; 
    public TextMeshProUGUI cuerpoUI; 

    private void Start()
    {
        // le pregunta al gamedata si ya habiamos pasado por esa y si si la destruye
        if (GameData.banderasCompletadas.Contains(idBandera))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // cuando el jugador toca la bandaera
        if (collision.CompareTag("Player"))
        {
            // guarda la posicion
            GameData.ultimoCheckpointPos = transform.position;
            GameData.tieneCheckpoint = true;
            GameData.PreguntaActualID = idBandera; // guarda el id para saber lo de la pregunta
            
            // pone que esta bandera ya la paso
            if (!GameData.banderasCompletadas.Contains(idBandera))
            {
                GameData.banderasCompletadas.Add(idBandera);
            }

            Time.timeScale = 0; // stop
            
            // 1. PRIMERO activamos el panel para que los textos "existan" en la escena
            winUI.SetActive(true); 

            // 2. LUEGO asignamos el texto
            tituloUI.text = "PREGUNTA " + idBandera;
            cuerpoUI.text = preguntaTexto;
            
        }
    }
}