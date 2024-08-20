using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Other;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Monsters
{
    public enum State { Patrolling, Chasing, Attacking }
    public class SkeletonBehaviour : NetworkBehaviour
    {
        private NavMeshAgent agent;
        public Vector2 target;
        public State currentState;
        private float attackRange = 1.5f;
        private float chaseRange = 15f;
        private float patrolSpeed = 2f;
        private float chaseSpeed = 4f;
        private float attackCooldown = 2f;
        private float lastAttackTime;
        public int difficultyLevel = 1; // 1 - прямое движение, 2 - зигзаг, 3 - по окружности
        private Vector2 patrolPoint;

        public override void OnNetworkSpawn()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            
            agent.stoppingDistance = 0.1f;
            currentState = State.Patrolling;
            SetRandomPatrolPoint();
        }

        private void Update()
        {
            switch (currentState)
            {
                case State.Patrolling:
                    getTarget();
                    Patrol();
                    break;
                case State.Chasing:
                    getTarget();
                    Chase();
                    break;
                case State.Attacking:
                    currentState = State.Patrolling;
                    break;
            }
        }

        private void Patrol()
        {
            if (Vector2.Distance(transform.position, patrolPoint) < 1f)
            {
                SetRandomPatrolPoint();
            }

            agent.SetDestination(patrolPoint);
        }

        private void getTarget()
        {
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, chaseRange);
            if (hitColliders.Where(x => x.CompareTag("Player")).ToList().Count == 0)
            {
                currentState = State.Patrolling;
                agent.speed = patrolSpeed;
                return;
            }
            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Player"))
                {
                    target = (Vector2) hitCollider.gameObject.transform.position + hitCollider.gameObject.GetComponent<PlayerMovement>().movementPrediction*15;
                    currentState = State.Chasing;
                    agent.speed = chaseSpeed;
                    break;
                }
            }
        }

        private void Chase()
        {
            if (target != null)
            {
                Vector2 destination;
                switch (difficultyLevel)
                {
                    case 1:
                        destination = target;
                        break;
                    case 2:
                        // Движение зигзагом
                        destination = target + GetZigzagOffset(target);
                        break;
                    case 3:
                        // Движение по окружности
                        destination = GetCircularMovement(target);
                        break;
                    default:
                        destination = target;
                        break;
                }

                agent.SetDestination(destination);

                /*float distanceToTarget = Vector2.Distance(transform.position, target);
                if (distanceToTarget <= attackRange)
                {
                    currentState = State.Attacking;
                }*/
            }
        }

        private Vector2 GetZigzagOffset(Vector2 targetPosition)
        {
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float zigzagFactor = Mathf.Sin(Time.time * 5) * 5;
            Vector2 perpendicularDirection = new Vector2(direction.y, -direction.x);
            return perpendicularDirection * zigzagFactor;
        }

        private Vector2 GetCircularMovement(Vector2 targetPosition)
        {
            float circleRadius = 5f;
            float angle = Time.time * Mathf.PI;
            return targetPosition + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * circleRadius;
        }


        private void SetRandomPatrolPoint()
        {
            Vector2 randomDirection = Random.insideUnitCircle * 5f;
            patrolPoint = (Vector2)transform.position + randomDirection;
        }
    }
}
