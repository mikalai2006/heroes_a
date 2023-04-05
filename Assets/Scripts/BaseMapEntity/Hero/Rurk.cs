//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class Rurk : UnitBase {
//    [SerializeField] private AudioClip _someSound;

//    [SerializeField] private Animator _animator;

//    [SerializeField] private Transform _model;

//    [SerializeField] public DataHero _heroData;

//    protected override void Awake()
//    {
//        base.Awake();
//        _animator = GetComponentInChildren<Animator>();
//        _model = transform.Find("Model");
//    }

//    public void SetNextPosition(Vector3Int position)
//    {

//    }

//    public override void UpdateAnimate(Vector3Int startPosition, Vector3Int endPosition)
//    {
//        if (_animator != null && startPosition != Vector3Int.zero && endPosition != Vector3Int.zero)
//        {
//            if (startPosition.x > endPosition.x)
//            {
//                _model.transform.localScale = new Vector3(-1,1,1);
//            } else
//            {
//                _model.transform.localScale = new Vector3(1, 1, 1);
//            }

//            Vector3Int direction = endPosition - startPosition;
//            Debug.Log($"Animator change::: {direction}");
//            _animator.SetFloat("X", (float)direction.x);
//            _animator.SetFloat("Y", (float)direction.y);
//        }
//    }

//    //protected override void Start() {
//    //    base.Start();
//    //    // Example usage of a static system
//    //    // AudioSystem.Instance.PlaySound(_someSound);
//    //    Debug.Log($"Rurk Init as {typeUnit}");
//    //}

//    public override void InitUnit(ScriptableUnitBase data, Vector3Int pos)
//    {

//        base.InitUnit(data, pos);


//    }

//    //public override void ExecuteMove() {
//    //    // Perform tarodev specific animation, do damage, move etc.
//    //    // You'll obviously need to accept the move specifics as an argument to this function. 
//    //    // I go into detail in the Grid Game #2 video
//    //    base.ExecuteMove(); // Call this to clean up the move
//    //}

//}
