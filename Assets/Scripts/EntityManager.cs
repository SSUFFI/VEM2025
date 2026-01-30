using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
//using Unity.VisualScripting;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Inst { get; private set; }
    void Awake() => Inst = this;

    [SerializeField] GameObject entityPrefab;
    [SerializeField] GameObject damagePrefab;
    [SerializeField] List<Entity> myEntities;
    [SerializeField] List<Entity> otherEntities;
    [SerializeField] GameObject TargetPicker;
    [SerializeField] Entity myEmptyEntity;
    [SerializeField] Entity myBossEntity;
    [SerializeField] Entity otherBossEntity;

    const int MAX_ENTITY_COUNT = 6;
    public bool IsFullMyEntities => myEntities.Count >= MAX_ENTITY_COUNT && !ExistMyEmptyEntity;
    bool IsFullOtherEntities => otherEntities.Count >= MAX_ENTITY_COUNT;
    bool ExistTargetPickEntity => targetPickEntity != null;
    bool ExistMyEmptyEntity => myEntities.Exists(x => x == myEmptyEntity);
    int MyEmptyEntityIndex => myEntities.FindIndex(x => x == myEmptyEntity);
    bool CanMouseInput => TurnManager.Inst.myTurn && !TurnManager.Inst.isLoading;

    Entity selectEntity;
    Entity targetPickEntity;
    WaitForSeconds delay1 = new WaitForSeconds(2);
    WaitForSeconds delay2 = new WaitForSeconds(2);

    void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;
    }

    void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void OnTurnStarted(bool myTurn)
    {
        AttackableReset(myTurn);

        if (!myTurn)
            StartCoroutine(AICo());
    }

    void Update()
    {
        ShowTargetPicker(ExistTargetPickEntity);
    }


    IEnumerator AICo()
    {
        int safety = 20;

        while (safety-- > 0)
        {
            bool success = CardManager.Inst.TryPutCard(false);

            if (!success)
                break;

            yield return delay1;
        }
        yield return StartCoroutine(AIAttackCo());

        TurnManager.Inst.EndTurn();
    }


    void EntityAlignment(bool isMine)
    {
        float targetY = isMine ? -3.33f : 2.88f;
        var targetEntities = isMine ? myEntities : otherEntities;

        for (int i = 0; i < targetEntities.Count; i++)
        {
            float targetX = (targetEntities.Count - 1) * -2.4f + i * 5.2f;

            var targetEntity = targetEntities[i];
            targetEntity.originPos = new Vector3(targetX, targetY, 0);
            targetEntity.MoveTransform(targetEntity.originPos, true, 0.5f);
            targetEntity.GetComponent<Order>()?.SetOriginOrder(i);
        }
    }

    public void InsertMyEmptyEntity(float xPos)
    {
        if (IsFullMyEntities)
            return;

        if (!ExistMyEmptyEntity)
            myEntities.Add(myEmptyEntity);

        Vector3 emptyEntityPos = myEmptyEntity.transform.position;
        emptyEntityPos.x = xPos;
        myEmptyEntity.transform.position = emptyEntityPos;

        int _emptyEntityIndex = MyEmptyEntityIndex;
        myEntities.Sort((entity1, entity2) => entity1.transform.position.x.CompareTo(entity2.transform.position.x));
        if (MyEmptyEntityIndex != _emptyEntityIndex)
            EntityAlignment(true);
    }

    public void RemoveMyEmptyEntity()
    {
        if (!ExistMyEmptyEntity)
            return;

        myEntities.RemoveAt(MyEmptyEntityIndex);
        EntityAlignment(true);
    }

    public bool SpawnEntity(bool isMine, Item item, Vector3 spawnPos)
    {
        if (isMine)
        {
            if (IsFullMyEntities || !ExistMyEmptyEntity)
                return false;
        }
        else
        {
            if (IsFullOtherEntities)
                return false;
        }

        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        var entity = entityObject.GetComponent<Entity>();

        if (isMine)
            myEntities[MyEmptyEntityIndex] = entity;
        else
            otherEntities.Insert(Random.Range(0, otherEntities.Count), entity);

        entity.isMine = isMine;
        entity.Setup(item);
        EntityAlignment(isMine);

        return true;
    }

    public void EntityMouseDown(Entity entity)
    {
        if (!CanMouseInput)
            return;

        selectEntity = entity;
    }

    public void EntityMouseUp()
    {
        if (!CanMouseInput)
            return;

        if (selectEntity && targetPickEntity && selectEntity.attackable)
            Attack(selectEntity, targetPickEntity);

        selectEntity = null;
        targetPickEntity = null;
    }

    public void EntityMouseDrag()
    {
        if (!CanMouseInput || selectEntity == null)
            return;

        // other 타겟엔티티 찾기
        bool existTarget = false;
        foreach (var hit in Physics2D.RaycastAll(Utils.MousePos, Vector3.forward))
        {
            Entity entity = hit.collider?.GetComponent<Entity>();
            if (entity != null && !entity.isMine && selectEntity.attackable)
            {
                targetPickEntity = entity;
                existTarget = true;
                break;
            }
        }
        if (!existTarget)
            targetPickEntity = null;
    }

    void Attack(Entity attacker, Entity defender)
    {
        attacker.attackable = false;
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Sequence sequence = DOTween.Sequence()
            .Append(attacker.transform.DOMove(defender.originPos, 0.4f)).SetEase(Ease.InSine)
            .AppendCallback(() =>
            {               
                //SpawnDamage(defender.attack, attacker.transform);
                //SpawnDamage(attacker.attack, defender.transform);
            })
            .Append(attacker.transform.DOMove(attacker.originPos, 0.4f)).SetEase(Ease.OutSine)
            .OnComplete(() => AttackCallback(attacker, defender));
    }

    void AttackCallback(params Entity[] entities)
    {
        Entity attacker = entities[0];
        Entity defender = entities[1];

        attacker.GetComponent<Order>().SetMostFrontOrder(false);

        if (defender.isBossOrEmpty)
        {
            int damage = attacker.attack;

            SpawnDamage(damage, defender.transform);
            CardManager.Inst.DamageDeck(damage, defender.isMine, attacker);
            RemoveEntityIfDead(attacker);

            return;
        }

        int attackerDamage = defender.attack;
        int defenderDamage = attacker.attack;

        attacker.Damaged(attackerDamage);
        defender.Damaged(defenderDamage);

        SpawnDamage(defenderDamage, defender.transform);
        SpawnDamage(attackerDamage, attacker.transform);

        foreach (var entity in entities)
        {
            if (!entity.isDie || entity.isBossOrEmpty)
                continue;

            Entity killer = (entity == attacker) ? defender : attacker;

            GraveManager.Inst.RaiseEntityDiedInCombat(entity.ItemData, entity.isMine, killer, entity);

            GraveManager.Inst.AddToGrave(entity.ItemData, entity.isMine);

            if (entity.isMine)
                myEntities.Remove(entity);
            else
                otherEntities.Remove(entity);

            Sequence sequence = DOTween.Sequence()
                .Append(entity.transform.DOShakePosition(1.3f))
                .Append(entity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
                .OnComplete(() =>
                {
                    EntityAlignment(entity.isMine);
                    Destroy(entity.gameObject);
                });
        }
    }

    IEnumerator CheckBossDie()
    {
        yield return delay2;

    }


    void ShowTargetPicker(bool isShow)
    {
        TargetPicker.SetActive(isShow);
        if (ExistTargetPickEntity)
            TargetPicker.transform.position = targetPickEntity.transform.position;
    }

    void SpawnDamage(int damage, Transform tr)
    {
        if (damage <= 0)
            return;

        var damageComponent = Instantiate(damagePrefab).GetComponent<Damage>();
        damageComponent.SetupTransform(tr);
        damageComponent.Damaged(damage);
    }

    public void AttackableReset(bool isMine)
    {
        var targetEntites = isMine ? myEntities : otherEntities;
        targetEntites.ForEach(x => x.attackable = true);
    }

    IEnumerator AIAttackCo()
    {
        List<Entity> attackers = new List<Entity>();
        for (int i = 0; i < otherEntities.Count; i++)
        {
            var e = otherEntities[i];
            if (e == null) continue;
            if (e.isDie) continue;
            if (e.isBossOrEmpty) continue;
            if (!e.attackable) continue;
            attackers.Add(e);
        }

        for (int i = 0; i < attackers.Count; i++)
        {
            int r = UnityEngine.Random.Range(i, attackers.Count);
            (attackers[i], attackers[r]) = (attackers[r], attackers[i]);
        }

        WaitForSeconds aiAtkDelay = new WaitForSeconds(0.9f);

        foreach (var attacker in attackers)
        {
            if (attacker == null || attacker.isDie) continue;
            if (!attacker.attackable) continue;

            Entity target = PickRandomMyTarget();

            if (target == null)
                break;

            if (target == myBossEntity)
            {
                yield return StartCoroutine(AttackBossCo(attacker));
            }
            else
            {
                Attack(attacker, target);
                yield return aiAtkDelay;
            }
        }
    }

    Entity PickRandomMyTarget()
    {
        List<Entity> candidates = new List<Entity>();

        for (int i = 0; i < myEntities.Count; i++)
        {
            var e = myEntities[i];
            if (e == null) continue;
            if (e.isDie) continue;
            if (e.isBossOrEmpty) continue;
            candidates.Add(e);
        }

        if (myBossEntity != null && !myBossEntity.isDie)
            candidates.Add(myBossEntity);

        if (candidates.Count == 0) return null;
        return candidates[UnityEngine.Random.Range(0, candidates.Count)];
    }

    IEnumerator AttackBossCo(Entity attacker)
    {
        attacker.attackable = false;
        attacker.GetComponent<Order>()?.SetMostFrontOrder(true);

        Vector3 bossPos = myBossEntity.originPos;

        Sequence seq = DOTween.Sequence()
            .Append(attacker.transform.DOMove(bossPos, 0.35f).SetEase(Ease.InSine))
            .AppendCallback(() =>
            {
                SpawnDamage(attacker.attack, myBossEntity.transform);
            })
            .Append(attacker.transform.DOMove(attacker.originPos, 0.35f).SetEase(Ease.OutSine))
            .OnComplete(() =>
            {
                attacker.GetComponent<Order>()?.SetMostFrontOrder(false);

                CardManager.Inst.DamageDeck(attacker.attack, true, attacker);

                if (EntityManager.Inst != null)
                    EntityManager.Inst.RemoveEntityIfDead(attacker);
            });

        yield return seq.WaitForCompletion();
    }

    public void ShowDamage(int damage, Transform tr)
    {
        SpawnDamage(damage, tr);
    }

    public void RemoveEntityIfDead(Entity entity)
    {
        if (entity == null) return;
        if (!entity.isDie) return;
        if (entity.isBossOrEmpty) return;

        if (entity.isMine)
        {
            if (!myEntities.Contains(entity)) return;
            myEntities.Remove(entity);
        }
        else
        {
            if (!otherEntities.Contains(entity)) return;
            otherEntities.Remove(entity);
        }

        Sequence sequence = DOTween.Sequence()
            .Append(entity.transform.DOShakePosition(1.3f))
            .Append(entity.transform.DOScale(Vector3.zero, 0.3f)).SetEase(Ease.OutCirc)
            .OnComplete(() =>
            {
                EntityAlignment(entity.isMine);
                Destroy(entity.gameObject);
            });
    }
}
