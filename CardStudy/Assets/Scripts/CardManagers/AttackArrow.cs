using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackArrow : MonoBehaviour
{
    [SerializeField] private GameObject bodyPrefab;
    [SerializeField] private GameObject headPrefab;

    //���˰�Χ���Ԥ����
    [SerializeField] private GameObject topLeftFrame;
    [SerializeField] private GameObject topRightFrame;
    [SerializeField] private GameObject bottomLeftFrame;
    [SerializeField] private GameObject bottomRightFrame;

    private GameObject topLeftPoint;
    private GameObject topRightPoint;
    private GameObject bottomLeftPoint;
    private GameObject bottomRightPoint;

    private const int AttackArrowPartsNumber = 17;
    private readonly List<GameObject> arrow = new List<GameObject>(AttackArrowPartsNumber);

    private Camera mainCamera;

    [SerializeField] private LayerMask enemyLayer;
    private GameObject selectedEnemy;

    private bool isArrowEnabled;

    private void Start()
    {
        for (var i = 0; i < AttackArrowPartsNumber - 1; i++)
        {
            var body = Instantiate(bodyPrefab, gameObject.transform);
            arrow.Add(body);
        }

        var head = Instantiate(headPrefab, gameObject.transform);
        arrow.Add(head);

        foreach (var part in arrow)
        {
            part.SetActive(false);
        }

        //���ɰ�Χ���λ�õĶ���
        topLeftPoint = Instantiate(topLeftFrame, gameObject.transform);
        topRightPoint = Instantiate(topRightFrame, gameObject.transform);
        bottomLeftPoint = Instantiate(bottomLeftFrame, gameObject.transform);
        bottomRightPoint = Instantiate(bottomRightFrame, gameObject.transform);

        DisableSelectionBox();

        mainCamera = Camera.main;
    }

    public void EnableArrow(bool arrowEnabled)
    {
        isArrowEnabled = arrowEnabled;
        foreach(var part in arrow)
        {
            part.SetActive(arrowEnabled);
        }

        if (!arrowEnabled)
        {
            UnSelectEnemy();
        }

    }

    private void LateUpdate()
    {
        if(!isArrowEnabled)
        {
            return;
        }

        var mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        var mouseX = mousePos.x;
        var mouseY = mousePos.y;

        var hitInfo = Physics2D.Raycast(mousePos, Vector3.forward, Mathf.Infinity, enemyLayer);

        if(hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject != selectedEnemy ||
                selectedEnemy == null)
            {
                SelectEnemy(hitInfo.collider.gameObject);
            }
            
        }
        else
        {
            UnSelectEnemy();
        }

        const float centerX = -0.15f;
        const float centerY = -0.92f;

        //for(var i = 0;i<arrow.Count;i++)
        //{
        //    var part = arrow[i];
        //    part.transform.position = new Vector3(centerX, centerY + 0.40f * i, 0.0f);
        //}

        //���ڼ��㱴�������ߵĿ��Ƶ�
        var controlAx = centerX - (mouseX - centerX) * 0.3f;
        var controlAy = centerY + (mouseY - centerY) * 0.8f;
        var controlBx = centerX + (mouseX - centerX) * 0.1f;
        var controlBy = centerY + (mouseY - centerY) * 1.4f;

        for (var i = 0; i < arrow.Count; i++)
        {
            var part = arrow[i];

            //�������������ڱ����������ϵ�λ��
            //��ͬ�����ָ�������ֵ���õ���Ӧλ�õ�tֵ
            //Խ������β���֣�tֵԽС
            //Խ������ͷ���֣�tֵԽ��
            var t = (i + 1) * 1.0f / arrow.Count;

            //t��ƽ��
            var tt = t * t;

            //t������
            var ttt = tt * t;

            var u = 1.0f - t;

            //u��ƽ��
            var uu = u * u;

            //u������
            var uuu = uu * u;

            //����λ��
            //����ʹ�ñ�������������������ʽ
            //PO��P1��P2��P3�ĸ�����ƽ�������ά�ռ��ж��������η�����������
            //������ʼ��PO����P1������P2�ķ�������P3��һ�㲻�ᾭ��P1��P2;
            //��������ֻ���������ṩ������Ϣ��PO��P1֮��ļ�࣬������������ת������P2֮ǰ������P1����ġ������ж೤��
            //�������ߵĲ�����ʽΪ:
            //B(t) = P0 * (1 - t) * (1 - t) * (1 - t) + 3 * P1 * t * (1 - t) * (1 - t) + 3 * P2 * t * t * (1 - t) +
            //       P3 * t * t * t , 0 <= t <= 1
            //������ u = (1 - t), uu = (1 - t) * (1 - t), uuu = (1 - t) * (1 - t) * (1 - t)
            //tt = t * t��ttt = t * t * t���򻯺�õ�:
            //B(t) = PO * uuu + 3 * P1 * t * uu + 3 * P2 * tt * u + P3 * ttt , 0 <= t <= 1
            //�����P3����centerX / centerY��P0��mouseX / mouseY��P1��controlAx / controlAy��P2��controlBx / controlBy
            //arrowX�����µļ������X����ֵ��arrowY�����µļ������Y����ֵ

            var arrowX = uuu * centerX +
                3 * uu * t * controlAx +
                3 * u * tt * controlBx +
                ttt * mouseX;

            var arrowY = uuu * centerY +
                3 * uu * t * controlAy +
                3 * u * tt * controlBy +
                ttt * mouseY;

            arrow[i].transform.position = new Vector3(arrowX, arrowY, 0.0f);

            //�����������־���ͼƬ�ĳ���/�Ƕ�
            float directionX;
            float directionY;

            if(i > 0)
            {
                //����β���ֵķ������
                directionX = arrow[i].transform.position.x - arrow[i - 1].transform.position.x; 
                directionY = arrow[i].transform.position.y - arrow[i - 1].transform.position.y;
            }
            else
            {
                //��Լ�β���ֵķ������
                directionX = arrow[i + 1].transform.position.x - arrow[i].transform.position.x;
                directionY = arrow[i + 1].transform.position.y - arrow[i].transform.position.y;
            }

            part.transform.rotation = Quaternion.Euler(0.0f, 0.0f, -Mathf.Atan2(directionX, directionY) * Mathf.Rad2Deg);

            part.transform.localScale = new Vector3(
                1.0f - 0.03f * (arrow.Count - 1 - i),
                1.0f - 0.03f * (arrow.Count - 1 - i),
                0);
        }

    }

    private void SelectEnemy(GameObject gameObj)
    {
        selectedEnemy = gameObj;

        var boxCollider = selectedEnemy.GetComponent<BoxCollider2D>();
        var size = boxCollider.size;
        var offset = boxCollider.offset;

        //ͨ��BoxCollider �Ĵ�СSize��ƫ���� Offset���������Χ����ĸ��ǵ�λ��
        var topLeftLocal = offset + new Vector2(-size.x * 0.5f, size.y * 0.5f);
        var topLeftWorld = gameObj.transform.TransformPoint(topLeftLocal);
        var topRightLocal = offset + new Vector2(size.x * 0.5f, size.y * 0.5f);
        var topRightWorld = gameObj.transform.TransformPoint(topRightLocal);
        var bottomLeftLocal = offset + new Vector2(-size.x * 0.5f, -size.y * 0.5f);
        var bottomLeftWorld = gameObj.transform.TransformPoint(bottomLeftLocal);
        var bottomRightLocal = offset + new Vector2(size.x * 0.5f, -size.y * 0.5f);
        var bottomRightWorld = gameObj.transform.TransformPoint(bottomRightLocal);

        topLeftPoint.transform.position = topLeftWorld;
        topRightPoint.transform.position = topRightWorld;
        bottomLeftPoint.transform.position = bottomLeftWorld;
        bottomRightPoint.transform.position = bottomRightWorld;

        EnableSelectionBox();
    }

    private void UnSelectEnemy()
    {
        selectedEnemy = null;
        DisableSelectionBox();

        foreach (var arrowObject in arrow)
        {
            arrowObject.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.white;
        }
    }

    private void EnableSelectionBox()
    {
        topLeftPoint.SetActive(true);
        topRightPoint.SetActive(true);
        bottomLeftPoint.SetActive(true);
        bottomRightPoint.SetActive(true);

        foreach (var arrowObject in arrow)
        {
            arrowObject.GetComponent<SpriteRenderer>().material.color = UnityEngine.Color.red;
        }
    }

    private void DisableSelectionBox()
    {
        topLeftPoint.SetActive(false);
        topRightPoint.SetActive(false);
        bottomLeftPoint.SetActive(false);
        bottomRightPoint.SetActive(false);
    }
}
