#include <stack>
#include <iostream>
using namespace std;

struct TreeNode
{
    int val;
    TreeNode *left;
    TreeNode *right;
    TreeNode(int x) : val(x), left(nullptr), right(nullptr) {}
};

struct BalanceTreeNode : TreeNode
{
    bool bLeft;
    bool bRight;
    BalanceTreeNode(int x) : TreeNode(x), bLeft(false), bRight(false) {}
};

class Solution
{
public:
    bool isBalance(TreeNode *root)
    {
        return false;
    }

    void Traverse(TreeNode *root, void (*Operation)(TreeNode *node))
    {
        if (root)
        {
            Traverse(root->left, Operation);
            Traverse(root->right, Operation);
            Operation(root);
        }
    }

    void Traverse(TreeNode *root, BalanceTreeNode *Bnode, void (*Operation)(TreeNode *node, BalanceTreeNode *Bnode))
    {
        if (root)
        {
            Traverse(root->left, Bnode, Operation);
            Traverse(root->right, Bnode, Operation);
            Operation(root, Bnode);
        }
    }
};

int main()
{
    TreeNode *root = new TreeNode(1);
    root->left = new TreeNode(2);
    root->right = new TreeNode(3);
    root->left->left = new TreeNode(4);
    root->left->right = new TreeNode(5);
    root->right->left = new TreeNode(6);
    root->right->right = new TreeNode(7);

    BalanceTreeNode *Broot;

    Solution solution;

    solution.Traverse(root, Broot, [](TreeNode *node, BalanceTreeNode *Bnode) {
        cout << node->val << " " << endl;
        
    });
}
