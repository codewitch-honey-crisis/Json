<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Show.aspx.cs" Inherits="TmdbBrowser.Show" %>
<%@ assembly name="System.Core" %>
<%@ import namespace="System.Text" %>
<%@ import namespace="System.Collections.Generic" %>
<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>media\tv\<%=show.Name%></title>
	<link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css"
          rel="stylesheet" integrity="sha384-T8Gy5hrqNKT+hzMclPo118YTQO6cYprQmhrYwIiQ/3axmI1hQomh7Ud2hPOy8SP1"
          crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css?family=Abel" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Gruppo" rel="stylesheet" />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.3.1/jquery.min.js"></script>
	
    <style type="text/css">
      
.clearfix::after {
  content: "";
  clear: both;
  display: table;
}
	  body {
            color: white;
            font-family: Gruppo;
            font-weight: 600;
            font-size: 1.5em;
            width: 100%;
            height: 100vh;
            width: 100%;
            background: #07051a;
        }
		
        header {
            font-family: 'Abel';
            text-decoration: none;
            color: white;
			background-color: rgba(7,7,26,.6);
            font-weight: 700;
            margin-bottom: 1em;
        }
		.darkbgtext {
            font-family: 'Gruppo';
            text-decoration: none;
            color: white;
			background-color: rgba(7,7,26,.6);
            font-weight: 700;
        }
		header span {
		            font-size: 1.5em;
		}

            header a:link {
                text-decoration: none;
                color: white;
            }

            header a:visited {
                text-decoration: none;
                color: white;
            }

            header a:hover {
                text-decoration: underline;
                color: white;
            }

            header a:active {
                text-decoration: none;
                color: white;
            }

        div.heading {
            opacity: .6;
            background-color: black;
            
        }

        section {
            text-align: left;
        }

            section.heading {
                text-align: left;
                padding: .2em 0em 0em .6em;
            }

        body {
            padding: 0;
            margin: 0;
            height: 100vh;
            width: 100%;
            background: #07051a;
        }
	.poster {
		float:left;
		padding-right: .5em;
		}
    		.hanging-indent
{
  
  text-indent : 2em;
  margin-left :  1em;

}
.resultlink {
 text-decoration: none;
}
.searchresult:hover {
 background: #17152a;
}

.blink {
  animation: blinker 1s step-start infinite;
}
@keyframes blinker {
  50% {
    opacity: 0;
  }
}
.btext {
margin-left:.5em;
margin-right:.5em;
}

.textstroke {
   color: white;
   text-shadow:
       3px 3px 0 #000,
     -1px -1px 0 #000,  
      1px -1px 0 #000,
      -1px 1px 0 #000,
       1px 1px 0 #000;
	   }
	   .searchform {
	   padding: 0 0 0 0;
		background-color:white;
	   }
	   .searchbox {
  border: none;
  margin-right: 0px;
  margin-top: -20px;
  margin-left: 10px;
  font-size: 17px;
  font-family: Gruppo;

	   }
	   .searchbox:focus {
		outline:none;
	   }
	   .searchbutton {
	   cursor: pointer;
	   border: 0px;
	   background: transparent;
	   color: black;

	   }
	  
<%if(null!=show.BackdropPath) {%>
body { 
  background: url('http://image.tmdb.org/t/p/original<%=show.BackdropPath%>') no-repeat center center fixed; 
  -webkit-background-size: cover;
  -moz-background-size: cover;
  -o-background-size: cover;
  background-size: cover;
}
	 div.gallery {
  margin: 5px;
  border: 1px solid #ccc;
  float: left;
  width: 182px;
}

div.gallery:hover {
  border: 1px solid #777;
}

.gallery_img {
  width: 180px;
  height: 250px;
  object-fit:contain;
  color: white;
  margin: 0px;
  padding: 0px;
  background-color: rgba(7,7,26,.6);
}

div.desc {
  padding: 15px;
  text-align: center;
}
<%}%>
    </style>
	
</head>
<body>
    <header>
        <span>
            <a href="Default.aspx">media</a><a href="SearchShows.aspx">\tv</a><a href="#">\<%=show.Name%></a>
        </span>
        <div style="float:right; display: inline-block;">
            <form action="SearchShows.aspx" method="GET" class="searchform w3-round-xlarge">
                <input class="searchbox" name="q" type="search" placeholder="&lt;search&gt;"/>
				<button class="searchbutton" type="submit"><i class="fa fa-search"></i></button>
			</form>
        </div>
		<div style="width:100%;text-align:center;">
			<div style="display:inline-block;">Runtime is <%=Math.Round(ApproxRunTime.TotalHours)%> hours</div>
			<div style="float:right; font-family:Gruppo"><%
						if(0<show.VoteCount) { %>
						<div class="w3-grey w3-round-xlarge" style="width:200px;">
							<div class="w3-container w3-round-xlarge w3-white" style=" width:<%=(int)(show.VoteAverage*10)%>%"><%=show.VoteAverage%>/10&nbsp;(<%=show.VoteCount%>&nbsp;vote<%=1!=show.VoteCount?"s":""%>)</div>
						</div>
						<% } else {%>
						<div class="w3-grey w3-round-xlarge" style="width:200px;">
							<div class="w3-container">(unrated)</div>
						</div>
						
						<%}%>
					</div>
		</div>
			
	
    </header>
    <main style="width:100%">
		<section class="header">
            <div class="btext">
				<span class="hanging-indent textstroke"><%=show.Overview%></span>
            </div>
		</section>
		<section>
			<%foreach (var season in show.Seasons){
				
			%>
			<div class="gallery">
  <a href="Season.aspx?show=<%=show.Id%>&season=<%=season.Number%>">
    <div class="gallery_img" style="background-size:contain; background-position-x:center; background-repeat:no-repeat; background-image:url('<%=TmdbApi.Tmdb.GetImageUrl(Denull(season.PosterPath,"/"),TmdbApi.Tmdb.Configuration.Images.PosterSizes[1])%>');"></div>
	
				<%var episodeCount = season.Episodes.Length; %>
 <div class="desc darkbgtext"><%=season.Name%><br /><%=episodeCount%> Episode<%=1!=episodeCount?"s":""%></div>
</a>
</div>
			
			<%}%>
			
		</section>
    </main>    
	
</body>
</html>
