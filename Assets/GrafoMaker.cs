using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class GrafoMaker : MonoBehaviour
{
    public List<GameObject> nodos;
    public Dictionary<GameObject, int> nodosId = new Dictionary<GameObject, int>();
    public Dictionary<int, GameObject> idNodos = new Dictionary<int, GameObject>();

    public TextAsset informacionGrafo;
    private string[] conexiones;
    public int intentos = 1;
    public float speed = 1;

    [Dropdown("nodos")]
    public GameObject NodoInicial;

    public List<Color> coloresDisponibles = new List<Color>();

    public Dictionary<GameObject, List<GameObject>> conexionesGrafo = new Dictionary<GameObject, List<GameObject>>();
    public Dictionary<GameObject, List<GameObject>> conexionesGrafo2 = new Dictionary<GameObject, List<GameObject>>();
    public List<GameObject> nodosVisitados = new List<GameObject>();
    public Dictionary<GameObject, int> coloresActuales = new Dictionary<GameObject,int>();
    public Dictionary<GameObject, Color> colorActual = new Dictionary<GameObject, Color>();

    public GameObject nodoAyuda;

    void Init(){
        foreach(var item in nodos){
            int actualValue;
            if (!nodosId.TryGetValue(item, out actualValue)){
                nodosId.Add(item, int.Parse(item.name));
                idNodos.Add(int.Parse(item.name), item);
            }
        }

        conexiones = informacionGrafo.ToString().Split(';');
        foreach (var item in conexiones)
        {
            string[] items = item.ToString().Split(' ');
            int temp1 = int.Parse(items[0]);
            int temp2 = int.Parse(items[1]);

            GameObject tempObject = new GameObject();
            tempObject.AddComponent<LineRenderer>();
            LineRenderer lineaTemp = tempObject.GetComponent<LineRenderer>();
            Vector3 pos1 = idNodos[temp1].transform.position;
            Vector3 pos2 = idNodos[temp2].transform.position;
            lineaTemp.positionCount = 2;
            lineaTemp.widthMultiplier = 0.2f;
            lineaTemp.SetPosition(0, pos1);
            lineaTemp.SetPosition(1, pos2);
            lineaTemp.startColor = Color.black;
            lineaTemp.endColor = Color.white;

            List<GameObject> actualValue;
            if (!conexionesGrafo.TryGetValue(idNodos[temp1], out actualValue)){
                conexionesGrafo.Add(idNodos[temp1], new List<GameObject>());
                conexionesGrafo2.Add(idNodos[temp1], new List<GameObject>());
            }
            if (!conexionesGrafo.TryGetValue(idNodos[temp2], out actualValue)){
                conexionesGrafo.Add(idNodos[temp2], new List<GameObject>());
                conexionesGrafo2.Add(idNodos[temp2], new List<GameObject>());
            }

            Color actualColor;
            if (!colorActual.TryGetValue(idNodos[temp1], out actualColor)){
                colorActual.Add(idNodos[temp1], Color.white);
                coloresActuales.Add(idNodos[temp1], 0);
            }
            if (!colorActual.TryGetValue(idNodos[temp2], out actualColor)){
                colorActual.Add(idNodos[temp2], Color.white);
                coloresActuales.Add(idNodos[temp2], 0);
            }

            conexionesGrafo[idNodos[temp1]].Add(idNodos[temp2]);
            conexionesGrafo2[idNodos[temp1]].Add(idNodos[temp2]);

            conexionesGrafo[idNodos[temp2]].Add(idNodos[temp1]);
            conexionesGrafo2[idNodos[temp2]].Add(idNodos[temp1]);

            Instantiate(tempObject, gameObject.transform);
        }

        // foreach(var conexiones in conexionesGrafo){
        //     foreach(var conexion in conexiones.Value){
        //         Debug.Log("Nodo: " + conexiones.Key.name + " conexion: " + conexion.name);
        //     }
        // }
    }

    private void OnEnable()
    {
        Init();
        StartCoroutine(RecorrerGrafos());
    }

    GameObject RecorrerGrafoss(GameObject _nodo,bool regreso)
    {
        GameObject nodoActual = _nodo;
        nodosVisitados.Add(nodoActual);

        List<GameObject> conexionesActuales = conexionesGrafo[nodoActual];
        List<GameObject> conexionesActuales2 = conexionesGrafo2[nodoActual];

        if(!regreso){
            if (PintarNodo(conexionesActuales2,nodoActual,Color.blue) && colorActual[nodoActual] != Color.blue){
                nodoActual.GetComponent<SpriteRenderer>().color = Color.blue;
                colorActual[nodoActual] = Color.blue;
            }
            else if (PintarNodo(conexionesActuales2,nodoActual,Color.red) && colorActual[nodoActual] != Color.red){
                nodoActual.GetComponent<SpriteRenderer>().color = Color.red;
                colorActual[nodoActual] = Color.red;
            }
            else if (PintarNodo(conexionesActuales2,nodoActual,Color.green) && colorActual[nodoActual] != Color.green){
                nodoActual.GetComponent<SpriteRenderer>().color = Color.green;
                colorActual[nodoActual] = Color.green;
            }
            else{
                return nodoAyuda;
            }
        }

        int nodoMayor = 14;
        GameObject siguienteNodo = nodoActual;
        foreach (var nodo in conexionesActuales)
        {
            if (nodosId[nodo] < nodoMayor)
            {
                if(!nodosVisitados.Exists(x => x == nodo)){
                    nodoMayor = nodosId[nodo];
                    siguienteNodo = idNodos[nodosId[nodo]];
                }
            }
        }

        // conexionesGrafo[nodoActual].Remove(siguienteNodo);
        // conexionesGrafo[siguienteNodo].Remove(nodoActual);
        Debug.Log("Nodo siguiente: " + siguienteNodo.name);
        return siguienteNodo;
    }

    IEnumerator RecorrerGrafos()
    {
        yield return new WaitForSeconds(2);
        GameObject siguiente = RecorrerGrafoss(NodoInicial,false);
        for(int i = 0;i<intentos;i++)
        {
            yield return new WaitForSeconds(speed);
            siguiente = RecorrerGrafoss(siguiente,false);
            if(siguiente == nodoAyuda){
                nodosVisitados.Remove(nodosVisitados[nodosVisitados.Count-1]);
                Debug.Log("Regreso al nodo: " + nodosVisitados[nodosVisitados.Count-1].name);
                siguiente = RecorrerGrafoss(nodosVisitados[nodosVisitados.Count-1],false);

            }else if(nodosVisitados[nodosVisitados.Count-1] == siguiente || siguiente == null){
                nodosVisitados.Remove(NodoInicial);
                siguiente = RecorrerGrafoss(NodoInicial,true);
            }
        }
    }

    bool PintarNodo(List<GameObject> _conexiones, GameObject _nodoActual, Color _color)
    {
        foreach (var conexion in _conexiones) {
            if (colorActual[conexion] == _color)
            {
                return false;
            }
        }

        return true;
    }
}
