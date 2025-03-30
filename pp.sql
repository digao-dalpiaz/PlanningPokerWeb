--
-- Table structure for table `erros`
--

DROP TABLE IF EXISTS `erros`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `erros` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data_hora` datetime NOT NULL,
  `ip` varchar(100) NOT NULL,
  `ident` varchar(200) NOT NULL,
  `detalhes` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `log`
--

DROP TABLE IF EXISTS `log`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `log` (
  `id` bigint(20) NOT NULL AUTO_INCREMENT,
  `data_hora` datetime NOT NULL,
  `ip` varchar(100) NOT NULL,
  `nome` varchar(100) NOT NULL,
  `sala` uuid NOT NULL,
  `modo` varchar(10) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=33 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_uca1400_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
