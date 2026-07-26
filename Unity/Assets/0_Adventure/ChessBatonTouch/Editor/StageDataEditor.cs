using UnityEngine;
using UnityEditor; // 에디터 전용 네임스페이스
using System.Linq;

// 이 스크립트가 StageDataSO의 인스펙터를 덮어씌운다는 선언
[CustomEditor(typeof(StageDataSO))]
public class StageDataEditor : Editor
{
    // 팔레트에서 선택한 설정 임시 저장
    private PieceType paintPiece = PieceType.None;
    private bool paintDisableStart = false;

    public override void OnInspectorGUI()
    {
        // 타겟 데이터를 StageDataSO로 캐스팅
        StageDataSO stageData = (StageDataSO)target;

        // --- 1. 팔레트 UI 영역 ---
        GUILayout.Label("🎨 팔레트 (선택 후 아래 보드에 클릭)", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical("box");
        paintPiece = (PieceType)EditorGUILayout.EnumPopup("그릴 기물", paintPiece);
        paintDisableStart = EditorGUILayout.Toggle("시작 불가(꼼수방지)", paintDisableStart);
        EditorGUILayout.EndVertical();

        GUILayout.Space(15);
        GUILayout.Label("♟️ 8x8 체스판 에디터", EditorStyles.boldLabel);

        // --- 2. 8x8 체스판 UI 영역 ---
        for (int y = 0; y < 8; y++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int x = 0; x < 8; x++)
            {
                // 현재 칸에 데이터가 있는지 확인
                PieceSetup currentSetup = stageData.initialPieces.FirstOrDefault(p => p.X == x && p.Y == y);

                // 버튼에 표시할 글자 설정
                string btnText = GetPieceSymbol(currentSetup?.Piece ?? PieceType.None);

                // 버튼 색상 설정 (빈칸: 기본, 일반기물: 초록, 시작불가: 회색)
                Color defaultColor = GUI.backgroundColor;
                if (currentSetup != null && currentSetup.Piece != PieceType.None)
                {
                    GUI.backgroundColor = currentSetup.DisableStart ? Color.gray : Color.green;
                }

                // 가로 40, 세로 40 크기의 버튼 생성
                if (GUILayout.Button(btnText, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    // Ctrl+Z(실행 취소)를 위한 기록 남기기
                    Undo.RecordObject(stageData, "Paint Chess Board");

                    // None을 칠하면(지우개) 리스트에서 제거
                    if (paintPiece == PieceType.None)
                    {
                        if (currentSetup != null) stageData.initialPieces.Remove(currentSetup);
                    }
                    else
                    {
                        // 기존에 데이터가 없으면 새로 생성해서 리스트에 추가
                        if (currentSetup == null)
                        {
                            currentSetup = new PieceSetup { X = x, Y = y };
                            stageData.initialPieces.Add(currentSetup);
                        }

                        // 팔레트에 선택된 데이터로 덮어쓰기
                        currentSetup.Piece = paintPiece;
                        currentSetup.DisableStart = paintDisableStart;
                    }

                    // 유니티에게 데이터가 변경되었음을 알려서 저장(Ctrl+S)되게 함
                    EditorUtility.SetDirty(stageData);
                }

                // 색상 복구
                GUI.backgroundColor = defaultColor;
            }
            EditorGUILayout.EndHorizontal();
        }

        GUILayout.Space(15);

        // 기본 리스트 형태도 보고 싶다면 아래 버튼을 눌러서 열 수 있도록 폴드아웃 추가
        // stageData.CorrectStartingPiece = (PieceSetup)EditorGUILayout.ObjectField("광고용 정답 기물 (나중에 추가)", null, typeof(PieceSetup), false);

        // 만약 기존의 List 형태 인스펙터도 그대로 아래에 띄우고 싶다면 주석 해제하세요
        // base.OnInspectorGUI(); 
    }

    // 기물 종류를 한 글자로 변환해 주는 헬퍼 함수
    private string GetPieceSymbol(PieceType type)
    {
        return type switch
        {
            PieceType.Pawn => "P",
            PieceType.Knight => "N",
            PieceType.Bishop => "B",
            PieceType.Rook => "R",
            PieceType.Queen => "Q",
            PieceType.King => "K",
            _ => "" // None일 때는 빈 칸
        };
    }
}