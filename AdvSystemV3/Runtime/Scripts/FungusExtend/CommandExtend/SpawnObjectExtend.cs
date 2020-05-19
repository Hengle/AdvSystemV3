// This code is part of the Fungus library (http://fungusgames.com) maintained by Chris Gregan (http://twitter.com/gofungus).
// It is released for free under the MIT open source license (https://github.com/snozbot/fungus/blob/master/LICENSE)

using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Spawns a new object based on a reference to a scene or prefab game object.
    /// </summary>
    [CommandInfo("Scripting", 
                 "Spawn Object (Extend)", 
                 "Spawns with more Parameter.", 
        Priority = 10)]
    [CommandInfo("GameObject",
                 "Instantiate (Extend)",
                 "Instantiate with more Parameter.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class SpawnObjectExtend : SpawnObject
    {
        [Tooltip("幾秒後刪掉這個Object, 0為不刪掉")]
        [SerializeField] protected float DeleteAfterSecond = 0.5f;
        [SerializeField] protected int sortLayer = 0;
        [SerializeField] protected int sortOrder = 0;


        public override void OnEnter()
        {
            if (_sourceObject.Value == null)
            {
                Continue();
                return;
            }

            GameObject newObject = null;

            if (_parentTransform.Value != null)
            {
                newObject = GameObject.Instantiate(_sourceObject.Value,_parentTransform.Value);
            }
            else
            {
                newObject = GameObject.Instantiate(_sourceObject.Value);
            }

            if (!_spawnAtSelf.Value)
            {
                newObject.transform.localPosition = _spawnPosition.Value;
                newObject.transform.localRotation = Quaternion.Euler(_spawnRotation.Value);
            }
            else
            {
                newObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }

            _newlySpawnedObject.Value = newObject;
            newObject.layer = sortLayer;

            var mpr = newObject.GetComponent<ParticleSystemRenderer>();
            if(mpr != null) mpr.sortingOrder = sortOrder;
            ParticleSystemRenderer[] pr = newObject.GetComponentsInChildren<ParticleSystemRenderer>();
            foreach (var item in pr)
            {
                item.sortingOrder = sortOrder;
            }

            if(DeleteAfterSecond > 0){
                Destroy(_newlySpawnedObject.Value, DeleteAfterSecond);
            }

            Continue();
        }

    }
}