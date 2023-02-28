clear
echo "Concerns Casework GitHub Release Utility"
echo "********************************************"
echo
echo "Before you begin, note down the following..."
echo "...The commit sha that should be tagged"
echo "...The release number you are going to use"
echo "...The branch you are tagging"
echo
echo "When choosing a release number use the format 'Major.Minor.Revision'" 
echo "If deploying to staging use a '-beta-xxx' suffix (and find out what the next beta number is first)"

echo
echo "Enter the release number being tagged."
read release_num
clear

echo
echo "Enter the commit SHA being tagged"
read commit_sha
clear

echo "Is this release being tagged on the 'main' branch (y/n)?"
read is_main

if [ $is_main == "y" ]
then
    $branch_name = "main"
else 
    echo
    echo "Enter the GitHub branch being tagged"
    read branch_name
fi
clear

echo
echo "Is this release for [s]taging or [p]roduction ?"
read environment
clear

echo
echo "-------------------------------- SCRIPT OUTPUT -------------------------------"
echo "--------------- Check this script is correct BEFORE you run it ---------------"
echo
echo

if [ $branch_name != "main" ]
then
    echo "git checkout main \"
    echo "git pull \"
fi

echo "git checkout $branch_name \"
echo "git pull \"

# tag the sha with the release number + environment
if [ $environment == "s" ]
then
    echo "git tag -a -m \"release $release_num\" $release_num $commit_sha \"
fi

if [ $environment == "p" ]
then
    echo "git tag -a -m \"release $release_num\" $release_num $commit_sha \"
fi
# push tags to server
echo "git push --tags"

echo
echo "--------------- After running this script sucessfully ---------------"
echo

if [ $environment == "s" ]
then
    echo "ON THE MAIN BRANCH: Update '.github\workflows\build-and-push-image-test.yml' and set it to deploy tag '$release_num'"
    echo "https://github.com/DFE-Digital/amsd-casework/blob/main/.github/workflows/build-and-push-image-test.yml"
fi

if [ $environment == "p" ]
then
    echo "ON THE MAIN BRANCH: Update '.github\workflows\build-and-push-image-production.yml' and set it to deploy tag '$release_num'"
    echo "https://github.com/DFE-Digital/amsd-casework/blob/main/.github/workflows/build-and-push-image-production.yml"
fi