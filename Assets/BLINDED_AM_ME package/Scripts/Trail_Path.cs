using UnityEngine;
using System.Collections;

namespace BLINDED_AM_ME{

	public struct Trail_Path_Point
	{
		public Vector3 point;
		public Vector3 forward;
		public Vector3 up;
		public Vector3 right;


		public Trail_Path_Point(Vector3 point, Vector3 forward, Vector3 up, Vector3 right)
		{
			this.point = point;
			this.forward = forward;
			this.up = up;
			this.right = right;
		}
	}

	public class Trail_Path : MonoBehaviour {
		
		public bool 		_isSmooth  = true;
		public bool         _isCircuit = false;
		private Transform[] _children_transforms;
		private Vector3[]   _points;
		private float[]     _distances;
		public float      	TotalDistance;

		// repeatedly used values
		Trail_Path_Point pathPoint = new Trail_Path_Point();

		float _interpolation = 0.0f;

		int[] _four_indices = new int[]{
			0,1,2,3
		}; 

		Vector3[] _four_points = new Vector3[]{
			Vector3.zero,
			Vector3.zero,
			Vector3.zero,
			Vector3.zero
		};

		void Awake(){

			if (transform.childCount > 1)
			{

				_children_transforms = new Transform[transform.childCount];
				for(int i=0; i< transform.childCount; i++){
					_children_transforms[i] = transform.GetChild(i);
					_children_transforms[i].gameObject.name = "point " + i;
				}

				GatherValues();
			}

		}

		public Trail_Path_Point GetPathPoint(float dist){

			if(_isCircuit)
				IsCircuit(dist);
			else
				IsNotCircuit(dist);

			return pathPoint;
		}

		private void IsNotCircuit(float dist){

			dist = Mathf.Clamp(dist, 0.0f, TotalDistance);

			// find segment index
			int index = 1;
			while (_distances[index] < dist){
				index++;
			}

			// the segment in the middle
			_four_indices[1] = index - 1;
			_four_indices[2] = index;

			_interpolation = Mathf.InverseLerp(
				_distances[_four_indices[1]],
				_distances[_four_indices[2]],
				dist);


			if (_isSmooth)
			{
				_four_indices[0] = Mathf.Clamp(index - 2, 0, _points.Length-1);
				_four_indices[3] = Mathf.Clamp(index + 1, 0, _points.Length-1);

				// assign the four points with the segment in the middle
				_four_points[0] = _points[_four_indices[0]];
				_four_points[1] = _points[_four_indices[1]];
				_four_points[2] = _points[_four_indices[2]];
				_four_points[3] = _points[_four_indices[3]];

				// you need two points to get a forward direction
				pathPoint.point = Math_Functions.CatmullRom(
					_four_points[0],
					_four_points[1],
					_four_points[2],
					_four_points[3],
					_interpolation);
				pathPoint.forward = Math_Functions.CatmullRom(
					_four_points[0],
					_four_points[1],
					_four_points[2],
					_four_points[3],
					_interpolation+0.01f) - pathPoint.point;

				pathPoint.forward.Normalize();
			}
			else // strait shooting
			{
				pathPoint.point = Vector3.Lerp(
					_points[_four_indices[1]], 
					_points[_four_indices[2]],
					_interpolation);

				pathPoint.forward = _points[_four_indices[2]] - _points[_four_indices[1]];
				pathPoint.forward.Normalize();
			}

			// 90 degree turn to right
			pathPoint.right = Vector3.Cross(
				Vector3.Lerp(
					transform.InverseTransformDirection(_children_transforms[_four_indices[1]].up),
					transform.InverseTransformDirection(_children_transforms[_four_indices[2]].up),
					_interpolation), // lerp
				pathPoint.forward).normalized; // cross

			// 90 degree turn to up
			pathPoint.up = Vector3.Cross(pathPoint.forward, pathPoint.right).normalized; 


			// now all points are 90 degrees from each other

		}

		private void IsCircuit(float dist){

			dist = dist % TotalDistance;

			// find segment index
			int index = 0;
			while (_distances[index] < dist){
				index++;
				if(index >= _distances.Length-1)
					break;
			}

			// the segment in the middle
			_four_indices[1] = ((index - 1) + _children_transforms.Length)%_children_transforms.Length;
			_four_indices[2] = index;

			_interpolation = Mathf.InverseLerp(
				_distances[_four_indices[1]],
				_distances[_four_indices[2]],
				dist);


			if (_isSmooth)
			{
				_four_indices[0] = ((index - 2) + _children_transforms.Length)%_children_transforms.Length;
				_four_indices[3] = (index + 1) % _children_transforms.Length;

				_four_indices[2] = _four_indices[2] % _children_transforms.Length;

				// assign the four points with the segment in the middle
				_four_points[0] = _points[_four_indices[0]];
				_four_points[1] = _points[_four_indices[1]];
				_four_points[2] = _points[_four_indices[2]];
				_four_points[3] = _points[_four_indices[3]];

				// you need two points to get a forward direction
				pathPoint.point = Math_Functions.CatmullRom(
					_four_points[0],
					_four_points[1],
					_four_points[2],
					_four_points[3],
					_interpolation);
				pathPoint.forward = Math_Functions.CatmullRom(
					_four_points[0],
					_four_points[1],
					_four_points[2],
					_four_points[3],
					_interpolation+0.01f) - pathPoint.point;

				pathPoint.forward.Normalize();
			}
			else // strait shooting
			{
				_four_indices[2] = _four_indices[2] % _children_transforms.Length;

				pathPoint.point = Vector3.Lerp(
					_points[_four_indices[1]], 
					_points[_four_indices[2]],
					_interpolation);

				pathPoint.forward = _points[_four_indices[2]] - _points[_four_indices[1]];
				pathPoint.forward.Normalize();
			}

			// 90 degree turn to right
			pathPoint.right = Vector3.Cross(
				Vector3.Lerp(
					transform.InverseTransformDirection(_children_transforms[_four_indices[1] % _children_transforms.Length].up),
					transform.InverseTransformDirection(_children_transforms[_four_indices[2] % _children_transforms.Length].up),
					_interpolation), // lerp
				pathPoint.forward).normalized; // cross

			// 90 degree turn to up
			pathPoint.up = Vector3.Cross(pathPoint.forward, pathPoint.right).normalized; 


			// now all points are 90 degrees from each other
		}

		private void GatherValues()
		{
			_points = new Vector3[ (_isCircuit ? _children_transforms.Length+1 : _children_transforms.Length)];
			_distances = new float[ (_isCircuit ? _children_transforms.Length+1 : _children_transforms.Length)];

			TotalDistance = 0.0f;

			for (int i=0; i<_points.Length-1; ++i){
				_points[i] = _children_transforms[i % _children_transforms.Length].localPosition;
				_distances[i] = TotalDistance;
				TotalDistance += Vector3.Distance(
					_points[i],
					_children_transforms[(i+1) % _children_transforms.Length].localPosition);
			}

			_points[_points.Length-1] = _children_transforms[_children_transforms.Length-1].localPosition;
			_distances[_points.Length-1] = TotalDistance;

		}

		#region Gizmo

		private void OnDrawGizmos()
		{
			DrawGizmos(false);
		}


		private void OnDrawGizmosSelected()
		{
			DrawGizmos(true);
		}


		private void DrawGizmos(bool selected)
		{

			_children_transforms = new Transform[transform.childCount];
			for(int i=0; i< transform.childCount; i++){
				_children_transforms[i] = transform.GetChild(i);
				_children_transforms[i].gameObject.name = "point " + i;
			}


			if (_children_transforms.Length > 1)
			{
				GatherValues();
			}

			if(_children_transforms.Length <= 1)
				return;


			Trail_Path_Point prev = GetPathPoint(0.0f);
			float dist = -1.0f;
			do{

				dist = Mathf.Clamp(dist + 10.0f,0,TotalDistance);

				Trail_Path_Point next = GetPathPoint(dist);

				Gizmos.color = selected ? new Color(0, 1, 1, 1) : new Color(0, 1, 1, 0.5f);
				Gizmos.DrawLine(transform.TransformPoint(prev.point), transform.TransformPoint(next.point));
				Gizmos.color = selected ? Color.green : new Color(0, 1, 0, 0.5f);
				Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.up*5.0f));
				Gizmos.color = selected ? Color.red : new Color(1, 0, 0, 0.5f);
				Gizmos.DrawLine(transform.TransformPoint(next.point), transform.TransformPoint(next.point) + transform.TransformDirection(next.right * 10.0f));

				prev = next;

			}while(dist < TotalDistance);

		}

		#endregion

	}
}
