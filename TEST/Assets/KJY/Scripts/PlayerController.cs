using UnityEngine;

public class PlayerController : MonoBehaviour
{
    float speed = 5f;       

    Vector3 targetPos;      

    Camera mainCamera;

    public float padding = 0.05f;       //UV좌표는 0~1사이, 작은값으로 수치 조정해야 함
    //UI요소 중 마진과 패딩이 있음
    //마진은 가장 바깥, 그 안쪽이 패딩, 중앙에 컨텐츠가 있음

    /// <summary>
    /// 플레이어 이동 구현하기
    /// 1. Transform.postion
    /// 2. Transform.Translate()
    /// 3. Vector3.MoveTowards()
    /// 4. 마우스 클리으로 이동
    /// 5. Rigidbody를 사용해서 이동
    /// </summary>

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //초기 목표 위치를 현재 위치로 설정
        targetPos = transform.position;

        //메인 카메라 참조 가져오기
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        //float ver = Input.GetAxis("Vertical");
        //float hor = Input.GetAxis("Horizontal");
        //Vector3 pos = new Vector3(hor , 0 , ver);

        //transform.position += pos * speed * Time.deltaTime;

        //플레이어 이동
        //Move1();
        //Move2();
        //Move3();
        Move4();

        MoveInScreen();

    }

    void Move1()
    {
        // 1. 입력 받기 (키보드 , 마우스 등 입력은 Input 매니저가 담당)
        // Input.GetAxis("Horizontal") 실수
        // Input.GetAxisRaw("Horizontal") 정수

        float ver = Input.GetAxis("Vertical");      //앞뒤
        float hor = Input.GetAxis("Horizontal");    //좌우
        Vector3 pos = new Vector3(hor, 0, ver);

        pos.Normalize();

        transform.position += pos * speed * Time.deltaTime;

    }

    void Move2()
    {
        float ver = Input.GetAxis("Vertical");      //앞뒤
        float hor = Input.GetAxis("Horizontal");    //좌우
        Vector3 pos = new Vector3(hor, 0, ver);

        pos.Normalize();

        //Translate는 로컬좌표계 기준 이동
        //transform.Translate(pos * speed * Time.deltaTime);
        //Space.Self 로컬좌표계 기준 이동 (오브젝트 자신의 좌표 - 자신의 방향 기준)
        
        transform.Translate(pos*speed*Time.deltaTime, Space.World);
        //Space.World 월드좌표계 기준 이동 (게임 세계의 절대 좌표)

    }

    void Move3()
    {
        //MoveTowards는 현재 위치에서 목표 위치로 일정 속도로 이동
        //MoveTowards(현재위치, 목표위치. 속도*시간)

        //1.입력 받기
        float ver = Input.GetAxis("Vertical");      //앞뒤
        float hor = Input.GetAxis("Horizontal");    //좌우 

        //2.입력이 있ㄷ으면 목표 위치 갱신
        if (ver != 0 || hor != 0)
        {
            Vector3 pos = new Vector3(hor, 0 , ver);
            pos.Normalize();

            targetPos += pos * speed * Time.deltaTime;
        }

        //3. 현재 위치에서 목표 위치로 이동
        transform.position =  Vector3.MoveTowards(transform.position, targetPos, speed);

    }

    void Move4()
    {
        //메인 카메라의 중요 함수
        //메인 카메라 자주 사용하지 않기때문에 어디서든 접근할 수 있도록 변수 선언해서 사용한다
        //Camera.main 씬에서 MainCamera 태그가 붙은 카메라를 찾아서 반환

        // 1.ScreenToWorldPoint : 화면 좌표를 월드 좌표로 변환
        // 2.ScreenToViewPoint : 화면 좌표를 뷰포트 좌표로 변환
        // 3.ViewportToWorldPoint : 뷰포트 좌푤르 월드 좌표로 변환
        // 4.WorldToScreenPoint : 월드 좌표를 화면 좌표로 변환
        // 5.WolrdToViewportPoint : 월드 좌표를 화면 좌표로 변환 
        // 6.ViewPortToScreenPoint : 뷰포트 좌푤르 화면 좌표로 변환

        //스크린의 화면을 마우스로 클릭했을 때 3D공간의 클릭지점으로 오브젝트를 움직일때
        //Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetMouseButton(0))
        {
            //마우스 스크린 좌푤르 월드 좦료로 변환
            //플레이어 높이는 유지해줘야 한다(z축)
            Vector3 mousePos = Input.mousePosition;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            worldPos.z = transform.position.z;

            print("마우스 클릭 좌표 : " + mousePos);
            print("월드 좌표 : " +  worldPos);
            print("플레이어 좌표  " + transform.position);

            // 만약 원 클릭으로 클릭좌표까지 이동하려면 아래 MoveTowards 함수가 업데이트함
            transform.position = Vector3.MoveTowards(transform.position, worldPos, speed * Time.deltaTime);
        }
    }

    void MoveInScreen()
    {
        //1. 화면밖 공간에 큐브 4개 만들어서 배치하면 충돌체 때문에 밖으로 벗어나지 못함
        //2. 플레이어 트랜스폼의 포지션 x,y 값을 고정시킴 Mathf.Clamp()
        //3. 메인카메라의 뷰포트를 가져와서 처리
        //스크린좌표 : 모니터 해상도의 픽셀
        //뷰포트좌표 : 카메라의 사각뿔 끝에 있는 사각형 왼쪽하단(0,0), 우측상단(1,1)
        //UV좌표 : 화면 텍스트, 2D 이미지를 표시하기 위한 좌표계 (텍스쳐좌표계라고 함) - 왼쪽상단(0,0), 우측하단(1,1)

        Vector3 position = mainCamera.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0f + (padding * 0.5f), 1f - (padding * 0.5f));
        position.y = Mathf.Clamp(position.y, 0f + padding, 1f - padding);

        transform.position = mainCamera.ViewportToWorldPoint(position);
    }
}
