using System.Collections;
using UnityEngine;

public class InfectionAttack : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameObject infectionBlobPrefab;
    [SerializeField] private GameObject PlayerObject;
    [SerializeField] private Pathfinder pathfinder;
    [SerializeField] private BigLou bigLou;

    [Header("Attack Parameters")]
    [SerializeField] private int numberOfBlobs = 8;
    [SerializeField] private float coneAngle = 60f;
    [SerializeField] private float attackPrepTime = 0.5f;
    [SerializeField] private float blobDuration = 2;

    [Header("Flight Parameters (Without Rigidbody)")]
    [SerializeField] private float flightDuration = 1.0f; 
    [SerializeField] private float apexHeight = 5f;

    public IEnumerator PerformInfectionAttack()
    {
        PlayerObject = pathfinder.GetPlayerObject();
        // 1. Innehalten / Aufladezeit
        yield return new WaitForSeconds(attackPrepTime);

        // 2. Blobs Spawnen und Werfen (ALLE IM GLEICHEN FRAME)
        for (int i = 0; i < numberOfBlobs; i++)
        {
            // 2a. Grundrichtung (Vektor vom Boss zum Spieler)
            Vector3 targetDirection = (PlayerObject.transform.position - transform.position);
            targetDirection.Normalize();

            // 2b. Zuf�lligen Winkel bestimmen und Richtung rotieren
            float halfCone = coneAngle / 2f;
            float randomAngleOffset = Random.Range(-halfCone, halfCone);
            Quaternion rotation = Quaternion.Euler(0, 0, randomAngleOffset);
            Vector3 finalDirection = rotation * targetDirection;

            // 2c. Das finale Ziel (wo der Blob landen soll)
            // Hier wird die Distanz zur Spielerposition projiziert, um das Ziel zu bestimmen.
            // Wir nehmen an, die Blobs fliegen eine feste Distanz, die etwa der Distanz zum Spieler entspricht.
            float distanceToPlayer = Vector3.Distance(transform.position, PlayerObject.transform.position);

            // Wir setzen das Ziel in Richtung finalDirection und auf die Distanz zum Spieler
            Vector3 landingTarget = transform.position + finalDirection * distanceToPlayer;

            // Da es 2D ist, setzen wir die Z-Koordinate auf die des Bosses
            landingTarget.z = transform.position.z;

            // 2d. Blob spawnen
            GameObject blob = Instantiate(infectionBlobPrefab, transform.position, Quaternion.identity);

            // 2e. Starte die separate Flug-Coroutine f�r diesen Blob
            StartCoroutine(MoveBlobInArc(blob.transform, landingTarget));
        }
        bigLou.currentState = BigLou.BossState.Resting;
        // Wichtig: Der Boss geht in den Resting-Zustand �ber, BEVOR die Blobs landen.
        // Wenn BigLou warten soll, bis alle Blobs gelandet sind, m�ssten wir hier warten.
        // Da dies ein Feuersto� ist, lassen wir ihn hier den Zustand wechseln.
    }

    private IEnumerator MoveBlobInArc(Transform blobTransform, Vector3 target)
    {
        Vector3 start = blobTransform.position;
        float timeElapsed = 0f;

        while (timeElapsed < flightDuration)
        {
            float t = timeElapsed / flightDuration;

            // 1. Horizontale Bewegung: Linear von Start zu Ziel
            Vector3 position = Vector3.Lerp(start, target, t);

            // 2. Vertikale Bewegung (Bogen): Parabel simulieren
            // Eine Funktion, die bei t=0 und t=1 Null ist und bei t=0.5 maximal ist: (4 * t * (1 - t))
            float arcFactor = 4 * t * (1 - t);

            // F�ge die Bogenh�he zur Y-Position hinzu
            position.y += arcFactor * apexHeight;

            // Blob aktualisieren
            blobTransform.position = position;

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Am Ende des Bogens: Position exakt setzen und Blob zerst�ren
        blobTransform.position = target;
        yield return new WaitForSeconds(blobDuration);
        Destroy(blobTransform.gameObject);       
    }
}
