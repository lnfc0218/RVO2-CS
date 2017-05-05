using System.Collections.Generic;
using System;

namespace RVO
{
    internal class KdTree
    {
        private struct FloatPair
        {
            internal float _a;
            internal float _b;

            public FloatPair(float a, float b)
            {
                _a = a;
                _b = b;
            }

            public static bool operator <(FloatPair lhs, FloatPair rhs)
            {
                return (lhs._a < rhs._a || !(rhs._a < lhs._a) && lhs._b < rhs._b);
            }
            public static bool operator <=(FloatPair lhs, FloatPair rhs)
            {
                return (lhs._a == rhs._a && lhs._b==rhs._b) || lhs < rhs;
            }
            public static bool operator >(FloatPair lhs, FloatPair rhs)
            {
                return !(lhs<=rhs);
            }
            public static bool operator >=(FloatPair lhs, FloatPair rhs)
            {
                return !(lhs < rhs);
            }
        }

        private const int MAX_LEAF_SIZE = 10;

        private struct AgentTreeNode
        {
            internal int begin;
            internal int end;
            internal int left;
            internal float maxX;
            internal float maxY;
            internal float minX;
            internal float minY;
            internal int right;
        }


        private RVOAgent[] agents_;
        private AgentTreeNode[] agentTree_;

        internal void buildAgentTree()
        {
            if (agents_ == null || agents_.Length != Simulator.Instance.agents_.Count)
            {
                agents_ = new RVOAgent[Simulator.Instance.agents_.Count];
                for (int i = 0; i < agents_.Length; ++i)
                {
                    agents_[i] = Simulator.Instance.agents_[i];
                }

                agentTree_ = new AgentTreeNode[2 * agents_.Length];
                for (int i = 0; i < agentTree_.Length; ++i)
                {
                    agentTree_[i] = new AgentTreeNode();
                }
            }

            if (agents_.Length != 0)
            {
                buildAgentTreeRecursive(0, agents_.Length, 0);
            }
        }

        void buildAgentTreeRecursive(int begin, int end, int node)
        {
            agentTree_[node].begin = begin;
            agentTree_[node].end = end;

            Vector2 position = agents_[begin].Position;
            agentTree_[node].minX = agentTree_[node].maxX = position.x_;
            agentTree_[node].minY = agentTree_[node].maxY = position.y_;

            for (int i = begin + 1; i < end; ++i)
            {
                position = agents_[i].Position;
                agentTree_[node].maxX = Math.Max(agentTree_[node].maxX, position.x_);
                agentTree_[node].minX = Math.Min(agentTree_[node].minX, position.x_);
                agentTree_[node].maxY = Math.Max(agentTree_[node].maxY, position.y_);
                agentTree_[node].minY = Math.Min(agentTree_[node].minY, position.y_);
            }

            if (end - begin > MAX_LEAF_SIZE)
            {
                /* No leaf node. */
                bool isVertical = (agentTree_[node].maxX - agentTree_[node].minX > agentTree_[node].maxY - agentTree_[node].minY);
                float splitValue = (isVertical ? 0.5f * (agentTree_[node].maxX + agentTree_[node].minX) : 0.5f * (agentTree_[node].maxY + agentTree_[node].minY));

                int left = begin;
                int right = end;

                while (left < right)
                {
                    while (left < right && (isVertical ? agents_[left].Position.x_ : agents_[left].Position.y_) < splitValue)
                    {
                        ++left;
                    }

                    while (right > left && (isVertical ? agents_[right - 1].Position.x_ : agents_[right - 1].Position.y_) >= splitValue)
                    {
                        --right;
                    }

                    if (left < right)
                    {
                        RVOAgent tmp = agents_[left];
                        agents_[left] = agents_[right - 1];
                        agents_[right - 1] = tmp;
                        ++left;
                        --right;
                    }
                }

                int leftSize = left - begin;

                if (leftSize == 0)
                {
                    ++leftSize;
                    ++left;
                    ++right;
                }

                agentTree_[node].left = node + 1;
                agentTree_[node].right = node + 1 + (2 * leftSize - 1);

                buildAgentTreeRecursive(begin, left, agentTree_[node].left);
                buildAgentTreeRecursive(left, end, agentTree_[node].right);
            }
        }
			
        internal void computeAgentNeighbors(RVOAgent agent, ref float rangeSq)
        {
            queryAgentTreeRecursive(agent, ref rangeSq, 0);
        }

        void queryAgentTreeRecursive(RVOAgent agent, ref float rangeSq, int node)
        {
            if (agentTree_[node].end - agentTree_[node].begin <= MAX_LEAF_SIZE)
            {
                for (int i = agentTree_[node].begin; i < agentTree_[node].end; ++i)
                {
                    agent.insertAgentNeighbor(agents_[i], ref rangeSq);
                }
            }
            else
            {
                Vector2 agentPosition = agent.Position;
                float distSqLeft = RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left].minX - agentPosition.x_)) + RVOMath.sqr(Math.Max(0.0f, agentPosition.x_ - agentTree_[agentTree_[node].left].maxX)) + RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].left].minY - agentPosition.y_)) + RVOMath.sqr(Math.Max(0.0f, agentPosition.y_ - agentTree_[agentTree_[node].left].maxY));

                float distSqRight = RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right].minX - agentPosition.x_)) + RVOMath.sqr(Math.Max(0.0f, agentPosition.x_ - agentTree_[agentTree_[node].right].maxX)) + RVOMath.sqr(Math.Max(0.0f, agentTree_[agentTree_[node].right].minY - agentPosition.y_)) + RVOMath.sqr(Math.Max(0.0f, agentPosition.y_ - agentTree_[agentTree_[node].right].maxY));

                if (distSqLeft < distSqRight)
                {
                    if (distSqLeft < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left);

                        if (distSqRight < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right);
                        }
                    }
                }
                else
                {
                    if (distSqRight < rangeSq)
                    {
                        queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].right);

                        if (distSqLeft < rangeSq)
                        {
                            queryAgentTreeRecursive(agent, ref rangeSq, agentTree_[node].left);
                        }
                    }
                }

            }
        }

    }
}