using UnityEngine;

//　キャラクター毎のステータス
[CreateAssetMenu( menuName = "CreateCharacterStatus", fileName = "CharacterStatus")]
public class CharaStatus : ScriptableObject
{
    public string charaName;
    public int hp;
    public int mp;
    public float attackPower;
    public int speed;
}
