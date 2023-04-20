using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenerator.Mesh;
using Game;

namespace MapGenerator
{
    public class Form
    {
        public enum Action { add, remove };
        public Action action;
        public List<FormModifier> modifiers = new();
        public Form(Action action)
        {
            this.action = action;
        }
        public List<T> GetIncluded<T>(List<T> input) where T : Linkable<T>, IVertices
        {
            List<T> result = new();
            input.ForEach(delegate (T item)
            {
                if (item.Vertices().TrueForAll(delegate (Vertex vertex) { return IsIncludes(vertex); }))
                {
                    result.Add(item);
                }
            });
            modifiers.ForEach(delegate (FormModifier modifier)
            {
                result = modifier.Action(result);
            });
            return result;
        }
        public virtual bool IsIncludes(Vertex vertex)
        {
            return false;
        }

        public virtual Form Clone()
        {
            Form form = new(action);
            modifiers.ForEach(delegate (FormModifier modifier)
            {
                form.modifiers.Add(modifier.Clone());
            });
            return form;
        }
    }
    public class FormRect : Form
    {
        public Vector2 corner1, corner2;

        public FormRect(Vector2 corner1, Vector2 corner2, Action action = Action.add) : base(action)
        {
            this.corner1 = new(corner1.x - 0.5f, corner1.y + 0.5f);
            this.corner2 = new(corner2.x + 0.5f, corner2.y - 0.5f);
        }

        public override Form Clone()
        {
            FormRect clone = base.Clone() as FormRect;
            clone.corner1 = corner1;
            clone.corner2 = corner2;
            return clone;
        }

        public override bool IsIncludes(Vertex vertex)
        {
            return vertex.coordinate.x >= corner1.x && vertex.coordinate.x <= corner2.x && vertex.coordinate.z <= corner1.y && vertex.coordinate.z >= corner2.y;
        }
        
    }
    public class FormCircle : Form
    {
        public Vector2 center;
        public float radius;

        public FormCircle(Vector2 center, float radius, Action action = Action.add) : base(action)
        {
            this.center = center;
            this.radius = radius;
        }

        public override Form Clone()
        {
            FormCircle clone = base.Clone() as FormCircle;
            clone.center = center;
            clone.radius = radius;
            return clone;
        }

        public override bool IsIncludes(Vertex vertex)
        {
            float distance = Mathf.Sqrt(Mathf.Pow(center.x - vertex.coordinate.x, 2) + Mathf.Pow(center.y - vertex.coordinate.z, 2));
            return distance < radius;
        }
    }

    public class FormPath : Form
    {
        public Vector2 start, target;
        public List<Cell> path;
        public int heightModifier;
        public int dirModifier;

        public FormPath(Action action = Action.add) : base(action) { }
        public FormPath(Vector2 start, Vector2 target, int dirModifier = 0, Action action = Action.add) : base(action)
        {
            this.start = start;
            this.target = target;
            this.dirModifier = dirModifier;
            path = PathFinder.FindPath(start, target, dirModifier);
        }
        public override Form Clone()
        {
            FormPath clone = base.Clone() as FormPath;
            clone.path = path;
            return clone;
        }
        public override bool IsIncludes(Vertex vertex)
        {
            foreach (Cell cell in path)
            {
                if (
                    cell.c == vertex ||
                    cell.fl == vertex ||
                    cell.fr == vertex ||
                    cell.bl == vertex ||
                    cell.br == vertex
                ){
                    return true;
                }
            }
            return false;
        }
    }
    public class FormAll : Form
    {
        public FormAll(Action action = Action.add) : base(action)
        {
        }
        public override bool IsIncludes(Vertex vertex)
        {
            return true;
        }
    }
    public class FormHeight : Form
    {
        public enum Operator { inside, outside };
        public Operator op;
        public float h1, h2;

        public FormHeight(Operator op, float h1 = float.NegativeInfinity, float h2 = float.PositiveInfinity, Action action = Action.add) : base(action)
        {
            this.op = op;
            this.h1 = h1;
            this.h2 = h2;
        }

        public override bool IsIncludes(Vertex vertex)
        {
            bool result = false;
            switch (op)
            {
                case Operator.inside:
                    if (vertex.coordinate.y < h2 && vertex.coordinate.y > h1) result = true;
                    break;
                case Operator.outside:
                    if (vertex.coordinate.y > h2 || vertex.coordinate.y < h1) result = true;
                    break;
            }
            return result;
        }
    }
    public abstract class FormModifier
    {
        public virtual List<T> Action<T>(List<T> input) where T : Linkable<T>
        {
            return input;
        }

        public abstract FormModifier Clone();
    }
    public class FormModifierAddSiblings : FormModifier
    {
        private int count;
        private int counter = 0;

        public FormModifierAddSiblings(int count = 0)
        {
            this.count = count;
        }
        public override List<T> Action<T>(List<T> input)
        {
            List<T> source = new();
            source.AddRange(input);
            do
            {
                List<T> linked = new();
                foreach (T item in input)
                {
                    foreach (T sibling in item.links)
                    {
                        if (!source.Contains(sibling) && !linked.Contains(sibling))
                        {
                            linked.Add(sibling);
                        }
                    }
                }
                input.AddRange(linked);
                source.Clear();
                source.AddRange(linked);
                counter++;
            } while (counter <= count);
            return input;
        }

        public override FormModifier Clone()
        {
            return new FormModifierAddSiblings(count);
        }
    }
    public class WithForms<M> where M : WithForms<M>
    {
        protected Queue<Form> forms = new();
        public Queue<Form> Forms => new(forms);
        public List<T> Get<T>(List<T> input) where T : Linkable<T>, IVertices
        {
            List<T> result = new();
            while (forms.TryDequeue(out Form form))
            {

                if (form.action == Form.Action.add)
                {
                    result.AddRange(form.GetIncluded(input));
                }
                else if (form.action == Form.Action.remove)
                {
                    form.GetIncluded(input).ForEach(delegate (T item)
                    {
                        result.Remove(item);
                    });
                }

            };
            return result;
        }
        public M EnqueueForm(Form form)
        {
            forms.Enqueue(form);
            return this as M;
        }
    }
    public interface IVertices
    {
        public List<Vertex> Vertices();
    }
}
