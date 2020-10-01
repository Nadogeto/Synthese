using SFML.System;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Drawing;
using SFML.Graphics;


namespace Synthese
{
    public class Intersection
    {

        //crée les données de l'intersection (la distance entre l'origine et l'objet détecté, le point d'intersection et son vecteur normal)
        struct RaySphereIntersection
        {
            public float distance;
            public Vector3 point;
            public Vector3 normal;
        }

        //crée les données du ray (son point d'origine et sa direction)
        struct Ray
        {
            public Vector3 origin;
            public Vector3 direction; //always normalize
        }

        //crée les données de la sphère (l'emplacement de son centre et son radius)
        public struct Sphere
        {
            public Vector3 center;
            public float radius;

            public Sphere(Vector3 center, float radius)
            {
                this.center = center;
                this.radius = radius;
            }
        }

        //crée les données de la lumière (l'emplacement et son intensité)
        public struct Light
        {
            public Point position;
            public float intensity;

            public Light(Point position, float intensity)
            {
                this.position = position;
                this.intensity = intensity;
            }
        }



        private static IList<Sphere> geometry = new List<Sphere>() {
            new Sphere(new Vector3(-1, 1, 10), 2),
            new Sphere(new Vector3(1, -1, 4), 1),
            new Sphere(new Vector3(0, -255, 0), 250),
        };

        private static IList<Light> lights = new List<Light>() {
            new Light(new Point(0, 6, 0), 60),
            new Light(new Point(-2, 4, 1), 15),
            };



        //public static bool Intersect(Ray ray, out int sphereIndex, out float distance,
        //                     float minDistance = 0, float maxDistance = float.MaxValue)
        //{
        //    distance = maxDistance;
        //    sphereIndex = -1;

        //    for (int t = 0; t < geometry.Count; ++t)
        //    {
        //        float distToSphere;

        //        if (geometry[t].Intersect(ray, out distToSphere))
        //        {
        //            if ((minDistance <= distToSphere) && (distToSphere < distance))
        //            {
        //                distance = distToSphere;
        //                sphereIndex = t;
        //            }
        //        }
        //    }

        //    return sphereIndex != -1;
        //}


        //récupère le premier point d'intersection atteint par le ray
        RaySphereIntersection? IntersectionRaySphere(Ray ray, Sphere sphere)
        {

            Vector3 m = ray.origin - sphere.center;
            float b = Vector3.Dot(m, ray.direction);
            float c = Vector3.Dot(m, m) - sphere.radius * sphere.radius;

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f)
                return null;


            float discriminant = b * b - c;

            // A negative discriminant corresponds to ray missing sphere
            if (discriminant < 0.0f)
                return null;

            RaySphereIntersection result;

            // Ray now found to intersect sphere, compute smallest t value of intersection
            float t1 = -b - (float)Math.Sqrt(discriminant);
            float t2 = -b + (float)Math.Sqrt(discriminant);

            if (t1 >= 0)
                result.distance = t1;
            else if (t2 >= 0)
                result.distance = t2;
            else
                return null;

            result.point = ray.origin + result.distance * ray.direction;
            result.normal = Vector3.Normalize(result.point - sphere.center);



            return result;
        }


        //remplit chaque pixel d'une image avec une couleur (changeante selon si un objet a été détecté par le ray)
        public void Fill()
        {

            uint height = 1000;
            uint width = 1000;

            Image img = new Image(width, height, Color.Blue);

            //remplit chaque pixel (x, y)
            for (uint x = 0 ; x < img.Size.X; ++x)
            {
                //remplit chaque pixel (x, y)
                for (uint y = 0; y < img.Size.Y; ++y)
                {
                    var ray = new Ray { };
                    ray.origin = new Vector3(x, y, 0);
                    ray.direction = new Vector3(0, 0, 1);

                    var sphere = new Sphere { };
                    sphere.center = new Vector3(500, 500, 200);
                    sphere.radius = 100;

                    var intersection = IntersectionRaySphere(ray, sphere);
                    if (intersection != null)
                        img.SetPixel(x, y, Color.Red);

                    //début lumière

                    foreach (var light in lights)
                    {
                        Vector hitPointToLight = light.position - hitPoint;
                        float distanceToLight = Vector.Length(hitPointToLight);

                        Ray lightRay = new Ray(hitPoint, hitPointToLight);

                        float distanceToObstacle;
                        int unused;

                        if (!Intersect(lightRay, out unused, out distanceToObstacle, 1e-4f, distanceToLight))
                        {
                            // there is no obstacle, so this light is visible from the hit
                            // point. therefore, calculate the amount of light here...

                            // lighting term = sphere color * dot(light vector, normal) * intensity / distance^2
                            color += Math.Max(0, Vector.Dot(lightRay.direction, normal)) * light.intensity / (float)Math.Pow(distanceToLight, 2);
                        }
                    }

                }
            }

            img.SaveToFile("C:/Users/ntobbal/Desktop/imgtest.png");
            Console.WriteLine("générée");
        }
    }

}
