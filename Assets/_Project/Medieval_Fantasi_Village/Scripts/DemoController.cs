using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoController : MonoBehaviour
{
    public GameObject[] m_Buildings;
    public int m_Index;

    // Use this for initialization
    public void Switch(bool _Next)
    {
        m_Buildings[m_Index].SetActive(false);

        if (_Next)
        {
            m_Index++;

            if (m_Index >= m_Buildings.Length)
                m_Index = 0;
        }
        else
        {
            m_Index--;

            if (m_Index < 0)
                m_Index = m_Buildings.Length - 1;
        }

        m_Buildings[m_Index].SetActive(true);
    }
}
