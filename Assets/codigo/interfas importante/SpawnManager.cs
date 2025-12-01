using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class PuntoDeAparicion
{
    [Tooltip("Transform del punto de aparición")]
    public Transform punto;

    [Tooltip("Lista de objetos que pueden aparecer en este punto")]
    public GameObject[] objetosPermitidos;
}

public class SpawnManager : MonoBehaviour
{
    [Header("Puntos de aparición configurados")]
    [Tooltip("Cada punto puede tener su propia lista de objetos permitidos.")]
    public PuntoDeAparicion[] puntosDeAparicion;

    private List<GameObject> objetosUsados = new List<GameObject>();

    private void Start()
    {
        GenerarObjetos();
    }

    private void GenerarObjetos()
    {
        foreach (var puntoData in puntosDeAparicion)
        {
            if (puntoData.punto == null || puntoData.objetosPermitidos.Length == 0)
                continue;

            // Crear una lista temporal con los objetos permitidos no usados
            List<GameObject> candidatos = new List<GameObject>();

            foreach (var obj in puntoData.objetosPermitidos)
            {
                if (!objetosUsados.Contains(obj))
                    candidatos.Add(obj);
            }

            // Si no hay candidatos disponibles, se salta este punto
            if (candidatos.Count == 0)
                continue;

            // Elegir un objeto aleatorio de los candidatos
            GameObject objetoElegido = candidatos[Random.Range(0, candidatos.Count)];

            // Instanciar el objeto en el punto de aparición
            Instantiate(objetoElegido, puntoData.punto.position, puntoData.punto.rotation);

            // Registrar que ya fue usado
            objetosUsados.Add(objetoElegido);
        }
    }
}
