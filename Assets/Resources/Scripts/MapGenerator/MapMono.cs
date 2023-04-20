using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using Game;

namespace MapGenerator
{
    public class MapMono : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            Location location = Location.Instance;
            location.Init(new(101, 101), transform);
            //new Lift(-1,2).EnqueueForm(new FormRect(new(0, 7), new(3, 0))).To(location);

            //new Flat(Flat.Operator.min).EnqueueForm(new FormRect(new(0, 3), new(11, 2))).To(location);
            FormRect rect = new(new(0,10), new(10, 0));
            new Lift(0.3f).EnqueueForm(rect).To(location);
            new Lift(1).EnqueueForm(new FormRect(new(2, 8), new(8, 2))).To(location);
            //new Sharp().EnqueueForm(new FormAll()).To(location);
            //new Flat(Flat.Operator.min).EnqueueForm((new FormCircle(new(50, 50), 20))).To(location);


            new Zone(new ZoneData("Stone", ZoneData.Type.rock)).EnqueueForm(rect).To(location);
            location.Generate();
            transform.GetComponent<NavMeshSurface>().BuildNavMesh();
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
