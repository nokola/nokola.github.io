# Converts from blogML to Hugo .md file format. Will set last modified date on files and update image links
#
# To use:
# 1. Place this script in a file (e.g. _migrate.ps1) in the same folder as your BlogML.xml file and update INPUT below
# 2. Start Powershell with execution policy allowing scripts: powershell -ExecutionPolicy Unrestricted
# 3. Go to your BlogML.xml's folder (cd folderpath) 
# 4. Run: ./_migrate.ps1

# INPUT:
$blogMLFileName = "_nokola-blog-2017-12-28.BlogML.xml"
$textToRemoveRegex = "http://nokola.com/blog/image.axd\?picture=|/Blog/image.axd\?picture=" # updates image sources to point to Hugo local images

# FUNCTIONALITY:
$file_name = "{0}-{1}.md"
$post_template = 
@"
---
title: `"{0}`"
date: {1}
draft: false
featured_image: ""
---

{2}
"@
$xml = [xml](get-content $blogMLFileName)
$xml.blog.posts.post | % { 
  $name = $file_name -f "", ($_."post-url" -replace ".aspx", "")
  $name = $name -replace "-/blog/post/\d{4}/\d{2}/\d{2}/", ""
  $createdDate = [DateTime]::Parse($_."date-created")
  $value = $post_template -f $_.title.InnerText, $createdDate.ToString("o"), $_.content.InnerText

  # update image links
  $value = $value -replace $textToRemoveRegex, "/"
  
  # write file
  $file = New-Item -Path . -Name $name -Value $value -ItemType file -Force
  $file.LastWriteTime = $createdDate
  echo $file
}
