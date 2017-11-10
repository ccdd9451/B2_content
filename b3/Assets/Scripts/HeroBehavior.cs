using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TreeSharpPlus;
using UnityEngine;

public class HeroBehavior : MonoBehaviour
{
    public Transform DropLoc1;
    public Transform DropLoc2;
    public GameObject trafficLight;

    public Node GetBehavior(GameObject hero)
    {
        GameObject wallet = GameObject.Find("Wallet");
        GameObject Station = GameObject.Find("StationPoint");

        IList<Node> movements = new List<Node>();

        // Scene Start Light
        movements.Add(TreeUtils.TreeNodeTrace(
            TreeUtils.WaitUntilTrue(() => trafficLight.activeInHierarchy), 
            "trafficLight monitor", true  // tracing
                ));

        movements.Add(new SelectorParallel(
                // Get to the station
                new DecoratorLoop(new DecoratorForceStatus(RunStatus.Success,
                TreeUtils.TreeNodeTrace(
                hero.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
                    Val.V(() => Station.transform.position), Val.V(0.5f)),
                    "Navigation to Station", false))),
                // Wallet Droped on half way
                new Sequence(DropWallet(hero), new LeafWait(5000))
            ));

        // Went back to get wallet
        movements.Add(
                TreeUtils.TreeNodeTrace(
                hero.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
                Val.V(() => wallet.transform.position), Val.V(2.0f)),
                "Navigation to Wallet", true)
            );
        // Get wallet from other people
        movements.Add(WalletExchange(hero));
        // Shake hand to him
        movements.Add(HandShaking(hero));
        // Go back to station
        //movements.Add(hero.GetComponent<BehaviorMecanim>().Node_GoToUpToRadius(
        //        Val.V(() => Station.transform.position), Val.V(2.0f)));

        // Have a Rest
        movements.Add(new LeafWait(1000));

        return new Sequence(movements.ToArray());
    }
    private Node HandShaking(GameObject hero)
    {
        IList<Node> movements = new List<Node>();
        BodyMecanim bm = hero.GetComponent<BodyMecanim>();
        // Hand out aquire for shaking
        movements.Add(TreeUtils.LeafInvokeIEnum(bm.RightHandReach()));
        movements.Add(new LeafInvoke(() => bm.WaitingHandShake = true));
        movements.Add(new DecoratorInvert(TreeUtils.WaitUntilFalse(() => bm.WaitingHandShake)));
        movements.Add(new Sequence(
                TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.highHandPos.gameObject)),
                TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.lowHandPos.gameObject))
                ));
        movements.Add(new Sequence(
        TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.highHandPos.gameObject)),
        TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.lowHandPos.gameObject))
        ));
        movements.Add(new Sequence(
        TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.highHandPos.gameObject)),
        TreeUtils.LeafInvokeIEnum(bm.RightHandMove(bm.Reflist.lowHandPos.gameObject))
        ));
        movements.Add(new LeafInvoke(() => bm.WaitingHandShake = true));
        movements.Add(new DecoratorInvert(TreeUtils.WaitUntilFalse(() => bm.WaitingHandShake)));
        movements.Add(TreeUtils.LeafInvokeIEnum(bm.RightHandReturn()));

        return new Sequence(movements.ToArray());
    }
    private Node WalletExchange(GameObject participant)
    {
        GameObject wallet = GameObject.Find("Wallet");
        IEnumerator<RunStatus> releaseE = participant.GetComponent<BodyMecanim>().RightHandReach();

        return new Sequence(
            participant.GetComponent<BehaviorMecanim>().Node_HeadLookTurnFirst(
                Val.V(() => wallet.transform.position)
                ),
            TreeUtils.WaitUntilTrue(IsWalletDropped),
            TreeUtils.LeafInvokeIEnum(participant.GetComponent<BodyMecanim>().RightHandReach(wallet)),
            new LeafInvoke(() => participant.GetComponent<BodyMecanim>().RightHandClaim(wallet)),
            TreeUtils.LeafInvokeIEnum(participant.GetComponent<BodyMecanim>().RightHandReturn()),
            new LeafWait(1000),
            new LeafInvoke(() => participant.GetComponent<BodyMecanim>().PocketClaim(wallet))
            );
    }
    private Node DropWallet(GameObject hero)
    {
        Func<bool> shouldDrop = () =>
        {
            return Input.GetKey(KeyCode.R);
        };
        GameObject wallet = GameObject.Find("Wallet");
        GameObject ground = GameObject.Find("Ground");

        Func<RunStatus> dropWallet = () =>
        {
            wallet.transform.parent = null;
            wallet.GetComponent<Rigidbody>().isKinematic = false;
            return RunStatus.Success;
        };
        return new Sequence(
            TreeUtils.WaitUntilTrue(shouldDrop),
            new LeafInvoke(dropWallet)
            );
    }
    bool IsWalletDropped()
    {
        GameObject wallet = GameObject.Find("Wallet");
        //Debug.Log((wallet.transform.parent == ground.transform).ToString());
        return wallet.transform.parent == null;
    }
}
