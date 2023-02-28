clear
echo "Concerns Casework GitHub Release Utility"
echo "********************************************"
echo
echo "Enter the release number being tagged."
echo "* Use the format 'Major.Minor.Revision'" 
echo "* If deploying to staging use a '-beta-xxx' suffix"
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
echo "--------------- SCRIPT OUTPUT ---------------"
echo
echo

if [ $branch_name != "main" ]
then
    echo "git checkout main \\"
    echo "git pull \\"
fi

echo "git checkout $branch_name \\"
echo "git pull \\"

# tag the sha with the release number + environment
if [ $environment == "s" ]
then
    echo "git tag -a -m \"release $release_num\" $release_num $commit_sha \\"
fi

if [ $environment == "p" ]
then
    echo "git tag -a -m \"release $release_num\" $release_num $commit_sha \\"
fi
# push tags to server
echo "git push --tags"

echo
echo "--------------- After running this script sucessfully ---------------"
echo

if [ $environment == "s" ]
then
    echo "Update '.github\workflows\build-and-push-image-test.yml' and set it to deploy tag '$release_num'"
fi

if [ $environment == "p" ]
then
    echo "Update '.github\workflows\build-and-push-image-production.yml' and set it to deploy tag '$release_num'"
fi