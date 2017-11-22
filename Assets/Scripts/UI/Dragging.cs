using UnityEngine;
using System.Collections;

namespace AGrail
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Dragging : MonoBehaviour
    {
        private ParticleSystem particle;

        void Awake()
        {
            particle = GetComponent<ParticleSystem>();
        }

        // Update is called once per frame
        void Update()
        {
            if (particle.isStopped)
                Destroy(this.gameObject);
        }
    }
}

