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
        struct Sphere
        {
            public Vector3 center;
            public float radius;
        }

        //récupère le premier point d'intersection atteint par le ray
        RaySphereIntersection? IntersectionRaySphere(Ray ray, Sphere sphere)
        {

            Vector3 m = ray.origin - sphere.center;
            float b = Vector3.Dot(m, ray.direction);
            float c = Vector3.Dot(m, m) - sphere.radius * sphere.radius;

            // Exit if r’s origin outside s (c > 0) and r pointing away from s (b > 0)
            if (c > 0.0f && b > 0.0f)
                return null;


            float discr = b * b - c;

            // A negative discriminant corresponds to ray missing sphere
            if (discr < 0.0f)
                return null;

            RaySphereIntersection result;

            // Ray now found to intersect sphere, compute smallest t value of intersection
            float t1 = -b - (float)Math.Sqrt(discr);
            float t2 = -b + (float)Math.Sqrt(discr);

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
                }
            }

            img.SaveToFile("C:/Users/NadjetTOBBAL/Desktop/imgtest.png");
        }
    }

}
