using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KameraSc : MonoBehaviour
{
    public bool CantMoveableB, FieldCantMove;
    public float dragSpeed = 0.5f; // Sürükleme hassasiyeti
    private Vector3 dragOrigin;

    float minX = -6.5f, maxX = 7.5f;
    float minY = -5.5f, maxY = 4.5f;

    public void BoolChanger(bool situation)
    {
        FieldCantMove = situation;
    }

    void Update()
    {
        if (CantMoveableB || FieldCantMove)
            return;

        if (Input.GetMouseButtonDown(0)) // İlk tıklamada başlangıç noktasını kaydet
        {
            dragOrigin = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0)) // Tıklama devam ediyorsa kamerayı hareket ettir
        {
            Vector3 difference = Camera.main.ScreenToWorldPoint(dragOrigin) - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPosition = transform.position + difference;

            // Kamerayı sınırlarla kısıtla
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

            transform.position = newPosition;
            dragOrigin = Input.mousePosition; // Güncellenmiş sürükleme pozisyonu
        }
    }
}
