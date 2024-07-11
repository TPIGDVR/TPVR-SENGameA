using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.pattern
{
    public class PoolingPattern<T> where T : class //where it is a game object
    {
        private GameObject prefab;
        private Queue<T> queue;

        public PoolingPattern(GameObject prefab)
        {
            queue = new Queue<T>();
            this.prefab = prefab;
        }

        public void Init(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add();
            }
        }

        public void InitWithParent(int numberOfItems, Transform parent)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add(parent);
            }
        }

        public void InitWithParent(int numberOfItems, Transform parent, bool isGameObjectActive)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add(parent , isGameObjectActive);
            }
        }

        public void Add(bool activeState = false)
        {
            GameObject initObject = GameObject.Instantiate(prefab);
            initObject.SetActive(activeState);
            queue.Enqueue(initObject.GetComponent<T>());
        }

        public void Add(Transform parent , bool activeState = false)
        {
            GameObject initObject = GameObject.Instantiate(prefab, parent);
            initObject.SetActive(activeState);
            queue.Enqueue(initObject.GetComponent<T>());
        }

        public T Get()
        {
            if (queue.Count == 0)
            {
                Add();
            }
            var initObject = queue.Dequeue();
            return initObject;
        }

        public void Retrieve(T initObject)
        {
            queue.Enqueue(initObject);
        }

    }

    public class PoolingPatternBasic
    {
        private GameObject prefab;
        private Queue<GameObject> queue;

        public PoolingPatternBasic(GameObject prefab)
        {
            queue = new Queue<GameObject>();
            this.prefab = prefab;
        }

        public void Init(int numberOfItems)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add();
            }
        }

        public void InitWithParent(int numberOfItems, Transform parent)
        {
            for (int i = 0; i < numberOfItems; i++)
            {
                Add(parent);
            }
        }

        public void Add()
        {
            GameObject initObject = GameObject.Instantiate(prefab);
            initObject.SetActive(false);
            queue.Enqueue(initObject);
        }

        public void Add(Transform parent)
        {
            GameObject initObject = GameObject.Instantiate(prefab, parent);
            initObject.SetActive(false);
            queue.Enqueue(initObject);

        }

        public GameObject Get()
        {
            if (queue.Count == 0)
            {
                Add();
            }
            var initObject = queue.Dequeue();
            initObject.SetActive(true);
            return initObject;
        }

        public void Retrieve(GameObject initObject)
        {
            initObject.SetActive(false);
            queue.Enqueue(initObject);
        }

    }

}