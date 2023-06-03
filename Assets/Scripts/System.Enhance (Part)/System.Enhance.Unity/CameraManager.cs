using UnityEngine;

namespace System.Enhance.Unity
{

	public class CameraManager : MonoBehaviour
	{
		//Vector3[] pos = new Vector3[8];
		//调试
		public float backDistance;//后面距离
		public float upDistance;//上面距离
		public float up;
		private Transform player;
		private Vector3 targetPos;
		RaycastHit hitInfo;
		public int cameraRotateSensitivity;//摄像机旋转灵敏度
		public int x = 10;
		public float y = 0.8f;
		private float minDistance = 5f;
		private void Start()
		{
			player = gameObject.transform;
			transform.position = Vector3.Lerp(transform.position, player.position - player.forward * backDistance + player.up * upDistance, Time.deltaTime * cameraRotateSensitivity);
		}
		private void LateUpdate()
		{
			transform.position = Vector3.Lerp(transform.position, player.position - player.forward * backDistance + player.up * upDistance, Time.deltaTime * cameraRotateSensitivity);
			targetPos = player.position + Vector3.up * up;
			transform.LookAt(targetPos);
			Physics.Raycast(targetPos, transform.position - targetPos, out hitInfo, Vector3.Magnitude(transform.position - targetPos) + y);
			DebugChan.Log(Vector3.Magnitude(transform.position - targetPos) + y);
			DebugChan.Log(Vector3.Magnitude(transform.position - targetPos));
			if (hitInfo.collider != null)
			{
				if (hitInfo.collider.name != transform.gameObject.name)
				{
					transform.position = hitInfo.point + (targetPos - transform.position).normalized * y;
				}
			}
		}
	}
}