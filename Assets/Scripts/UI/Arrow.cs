using UnityEngine;
using System.Collections;

namespace AGrail
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Arrow : MonoBehaviour
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

        public void SetParms(Vector3 src, Vector3 dst)
        {
            var dir = dst - src;
            var q = Quaternion.FromToRotation(Vector3.up, dir);
            transform.localRotation = q;

            var dist = dir.magnitude;
            particle.startSpeed = dist / particle.startLifetime;
            particle.Play();
        }
    }
}

