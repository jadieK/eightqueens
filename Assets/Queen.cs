using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : MonoBehaviour
{
   public GameObject QueenChess;

   public void ModifyQueen(bool isActive)
   {
      QueenChess.SetActive(isActive);
   }
}
