<%@ Page Language="C#" CodeBehind="SearchShows.aspx.cs" Inherits="TmdbBrowser.SearchShows" %>
<%@ Import Namespace="Json" %>
<%@ Import Namespace="TmdbApi" %>

<!doctype html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <title>media</title>
	<link rel="stylesheet" href="https://www.w3schools.com/w3css/4/w3.css">
    <link href="https://maxcdn.bootstrapcdn.com/font-awesome/4.6.3/css/font-awesome.min.css" rel="stylesheet" integrity="sha384-T8Gy5hrqNKT+hzMclPo118YTQO6cYprQmhrYwIiQ/3axmI1hQomh7Ud2hPOy8SP1" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css?family=Abel" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css?family=Gruppo" rel="stylesheet" />
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
            opacity: .6;
            text-decoration: none;
            color: white;
            font-weight: 700;
            margin-bottom: 1em;
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

        form {
            position: relative;
            top: 50%;
            left: 50%;
            transform: translate(-50%,-50%);
            transition: all 1s;
            width: 50px;
            height: 50px;
            background: white;
            box-sizing: border-box;
            border-radius: 25px;
            border: 4px solid white;
            padding: 5px;
        }

        input {
            position: absolute;
            top: 0;
            left: 0;
            width: 100%;
            height: 42.5px;
            line-height: 30px;
            outline: 0;
            border: 0;
            display: none;
            font-size: 1em;
            border-radius: 20px;
            padding: 0 20px;
        }
		.poster {
		float:left;
		padding-right: .5em;
		}
        .fa {
            box-sizing: border-box;
            padding: 10px;
            width: 42.5px;
            height: 42.5px;
            position: absolute;
            top: 0;
            right: 0;
            border-radius: 50%;
            color: #07051a;
            text-align: center;
            font-size: 1.2em;
            transition: all 1s;
        }

        form:hover {
            width: 200px;
            cursor: pointer;
        }

            form:hover input {
                display: block;
            }

            form:hover .fa {
                background: #07051a;
                color: white;
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

    </style>
</head>
<body>
    <header>
        <span>
            <a href="Default.aspx">media</a><a href="SearchShows.aspx">\tv</a>
        </span>
        <div style="float:right;margin-top:30px;">
            <form action="" method="get">
                <input name="q" type="search" value="<%=search%>">
                <i class="fa fa-search"></i>
            </form>
        </div>
    </header>
    <main style="width:100%">
        <section class="heading"><%
		if(!string.IsNullOrEmpty(search)) {%>
		<span>Search &quot;<%=search%>&quot;</span><%
		} else {%>
		<span>Discover</span><%
		}%>
		</section>
		<div id="searchResults" style="margin-left: .2em; margin-right: .2em;padding-left: .6em;"><%
		if(null!=results)
			foreach(var result in results) {%>	
			<a href="./Show.aspx?show=<%=result.Id%>" class="resultlink">
			<div class="clearfix searchresult" style="margin-bottom: .5em;">
				<div>
					<span style="font-family: Abel;"><%=result.Name%></span>
					<div style="float:right;"><%
						if(0<result.VoteCount) { %>
						<div class="w3-grey w3-round-xlarge" style="width:200px;">
							<div class="w3-container w3-round-xlarge w3-white" style="width:<%=(int)(result.VoteAverage*10)%>%"><%=result.VoteAverage%>/10&nbsp;(<%=result.VoteCount%>&nbsp;vote<%=1!=result.VoteCount?"s":""%>)</div>
						</div>
						<% } else {%>
						<div class="w3-grey w3-round-xlarge" style="width:200px;">
							<div class="w3-container">(unrated)</div>
						</div>
						
						<%}%>
					</div>
				
				</div>
				<%if(null!=result.PosterPath) {%>
				<img class="poster" src="<%=Tmdb.GetImageUrl(Denull(result.PosterPath,"/#"))%>" width="150" />
				<%}%>
				<div class="hanging-indent">
					<p><%=result.Overview%></p>
				</div>
			</div>
			</a>
			<%}%>
		</div>
    </main>
</body>
</html>
